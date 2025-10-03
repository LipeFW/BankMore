using BankMore.Transfer.Domain.DTOs.Responses;
using BankMore.Transfer.Domain.Entities;
using BankMore.Transfer.Domain.Exceptions;
using BankMore.Transfer.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Text.Json;

namespace BankMore.Transfer.Application.Commands
{
    public class TransferHandler : IRequestHandler<TransferCommand>
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ITransferRepository _transferenciaRepository;
        private readonly IIdempotencyRepository _idempotencyRepository;

        public TransferHandler(IConfiguration configuration,
            IHttpContextAccessor httpContext,
            ITransferRepository transferenciaRepository,
            IIdempotencyRepository idempotencyRepository
)
        {
            _configuration = configuration;
            _httpContextAccessor = httpContext;
            _transferenciaRepository = transferenciaRepository;
            _idempotencyRepository = idempotencyRepository;
        }

        public async Task Handle(TransferCommand command, CancellationToken cancellationToken)
        {
            var transferExists = await _idempotencyRepository.GetByRequestIdAsync(command.IdRequisicao);

            if (transferExists != null)
            {
                throw new IdempotencyViolationException(command.IdRequisicao.ToString("d"));
            }

            if (command.Valor <= 0)
                throw new InvalidValueException("Apenas valores positivos podem ser recebidos.");

            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            var httpClient = new RestClient(_configuration["AccountAPI:BaseAddress"]);

            if (!string.IsNullOrEmpty(token))
                httpClient.AddDefaultHeader("Authorization", token);

            Guid destinationAccountId = Guid.Empty;

            // Débito conta de origem
            if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst("AccountNumber")?.Value, out var sourceAccountNumber))
            {
                var debitRequest = new RestRequest("api/accounts/movements", Method.Post).AddJsonBody(new
                {
                    requestId = Guid.NewGuid(),
                    accountNumber = sourceAccountNumber,
                    valor = command.Valor,
                    tipo = "D"
                });

                var debitResponse = await httpClient.ExecuteAsync(debitRequest);

                if (!debitResponse.IsSuccessStatusCode)
                {
                    var errorResponse = debitResponse.Content != null
                        ? JsonSerializer.Deserialize<ErrorResponse>(debitResponse.Content)
                        : null;

                    switch (errorResponse.ErrorType)
                    {
                        case "INVALID_ACCOUNT":
                            throw new InvalidAccountException(errorResponse.Message);
                        case "INACTIVE_ACCOUNT":
                            throw new InactiveAccountException(errorResponse.Message);
                        default:
                            throw new InvalidOperationException("Falha ao debitar da conta de origem");
                    }
                }
            }

            // Crédito conta de destino
            var creditRequest = new RestRequest("api/accounts/movements", Method.Post).AddJsonBody(new
            {
                requestId = Guid.NewGuid(),
                accountNumber = command.NumeroContaDestino,
                valor = command.Valor,
                tipo = "C"
            });

            var creditResponse = await httpClient.ExecuteAsync(creditRequest);

            // Se não tiver sucesso no crédito de destino, efetua o estorno (crédito na conta origem)
            if (!creditResponse.IsSuccessStatusCode)
            {
                var sourceCreditRequest = new RestRequest("api/accounts/movements", Method.Post).AddJsonBody(new
                {
                    requestId = Guid.NewGuid(),
                    accountNumber = sourceAccountNumber,
                    valor = command.Valor,
                    tipo = "C"
                });

                var sourceCreditResponse = await httpClient.ExecuteAsync(sourceCreditRequest);

                var errorResponse = creditResponse.Content != null
                    ? JsonSerializer.Deserialize<ErrorResponse>(creditResponse.Content)
                    : null;

                switch (errorResponse.ErrorType)
                {
                    case "INVALID_ACCOUNT":
                        throw new InvalidAccountException(errorResponse.Message);
                    case "INACTIVE_ACCOUNT":
                        throw new InactiveAccountException(errorResponse.Message);
                    default:
                        throw new InvalidOperationException("Falha ao creditar conta destino");

                }
            }

            destinationAccountId = creditResponse?.Headers?.FirstOrDefault(h => h.Name == "AccountId")?.Value != null
                  ? Guid.Parse(creditResponse.Headers.First(h => h.Name == "AccountId").Value)
                  : throw new InvalidOperationException("Falha ao obter ID da conta corrente de origem");

            // Persistir transferência
            var transferencia = new Transferencia
            {
                IdTransferencia = Guid.NewGuid(),
                IdContaCorrenteOrigem = command.IdContaOrigem,
                IdContaCorrenteDestino = destinationAccountId,
                Valor = command.Valor,
                DataTransferencia = DateTime.UtcNow
            };

            await _transferenciaRepository.AddAsync(transferencia);

            await _idempotencyRepository.AddAsync(new Idempotencia(command.IdRequisicao, JsonSerializer.Serialize(command), JsonSerializer.Serialize(transferencia)));

        }
    }
}
