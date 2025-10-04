using BankMore.Transfer.Domain.DTOs.Requests;
using BankMore.Transfer.Domain.DTOs.Responses;
using BankMore.Transfer.Domain.Entities;
using BankMore.Transfer.Domain.Exceptions;
using BankMore.Transfer.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RestSharp;
using System.Text.Json;
using System.Windows.Input;

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
                var debitRequest = new RestRequest("api/accounts/movements", Method.Post)
                    .AddJsonBody(new MovementAccountRequest(Guid.NewGuid(), sourceAccountNumber, command.Valor, "D"));

                var debitResponse = await httpClient.ExecuteAsync(debitRequest);

                if (!debitResponse.IsSuccessStatusCode)
                {
                    var errorResponse = debitResponse.Content != null
                        ? JsonSerializer.Deserialize<ErrorResponse>(debitResponse.Content)
                        : null;

                    switch (errorResponse?.ErrorType)
                    {
                        case "INVALID_ACCOUNT":
                            throw new InvalidAccountException($"Houve um problema ao debitar da conta de origem. {errorResponse.Message}");
                        case "INACTIVE_ACCOUNT":
                            throw new InactiveAccountException($"Houve um problema ao debitar da conta de origem. {errorResponse.Message}");
                        default:
                            throw new InvalidOperationException("Falha ao debitar da conta de origem");
                    }
                }
            }

            // Crédito conta de destino
            var creditRequest = new RestRequest("api/accounts/movements", Method.Post)
                .AddJsonBody(new MovementAccountRequest(Guid.NewGuid(), command.NumeroContaDestino, command.Valor, "C"));

            var creditResponse = await httpClient.ExecuteAsync(creditRequest);

            // Se não tiver sucesso no crédito de destino, efetua o estorno (crédito na conta origem)
            if (!creditResponse.IsSuccessStatusCode)
            {
                var sourceCreditRequest = new RestRequest("api/accounts/movements", Method.Post)
                    .AddJsonBody(new MovementAccountRequest(Guid.NewGuid(), sourceAccountNumber, command.Valor, "C"));

                var sourceCreditResponse = await httpClient.ExecuteAsync(sourceCreditRequest);

                var creditResponseDetails = creditResponse.Content != null
                    ? JsonSerializer.Deserialize<ErrorResponse>(creditResponse.Content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    })
                    : null;

                switch (creditResponseDetails?.ErrorType)
                {
                    case "INVALID_ACCOUNT":
                        throw new InvalidAccountException($"Houve um problema ao creditar a conta de destino. {creditResponseDetails.Message}");
                    case "INACTIVE_ACCOUNT":
                        throw new InactiveAccountException($"Houve um problema ao creditar a conta de destino. {creditResponseDetails.Message}");
                    default:
                        throw new InvalidOperationException("Falha ao creditar conta destino");
                }
            }

            destinationAccountId = creditResponse?.Headers?.FirstOrDefault(h => h.Name == "AccountId")?.Value != null
                  ? Guid.Parse(creditResponse.Headers.First(h => h.Name == "AccountId").Value)
                  : throw new InvalidOperationException("Falha ao obter ID da conta corrente de origem");

            // Persistir transferência
            var transfer = new Transferencia(command.IdContaOrigem, destinationAccountId, DateTime.UtcNow, command.Valor);

            await _transferenciaRepository.AddAsync(transfer);

            await _idempotencyRepository.AddAsync(new Idempotencia(command.IdRequisicao, JsonSerializer.Serialize(command), JsonSerializer.Serialize(transfer)));

        }
    }
}
