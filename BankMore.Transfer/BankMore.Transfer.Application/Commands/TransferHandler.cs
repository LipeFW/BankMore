using BankMore.Transfer.Domain.Entities;
using BankMore.Transfer.Domain.Exceptions;
using BankMore.Transfer.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using RestSharp;

namespace BankMore.Transfer.Application.Commands
{
    public class TransferHandler : IRequestHandler<TransferCommand>
    {
        private readonly ITransferRepository  _transferenciaRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public TransferHandler(ITransferRepository transferenciaRepository,
            IHttpContextAccessor httpContext,
            IConfiguration configuration)
        {
            _transferenciaRepository = transferenciaRepository;
            _httpContextAccessor = httpContext;
            _configuration = configuration;
        }

        public async Task Handle(TransferCommand command, CancellationToken cancellationToken)
        {
            if(command.Valor <= 0)
                throw new InvalidValueException("Apenas valores positivos podem ser recebidos.");

            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            var httpClient = new RestClient(_configuration["AccountAPI:BaseAddress"]);
            
            if (!string.IsNullOrEmpty(token))
                httpClient.AddDefaultHeader("Authorization", token);

            Guid destinationAccountId = Guid.Empty;

            if (int.TryParse(_httpContextAccessor.HttpContext.User.FindFirst("AccountNumber")?.Value, out var sourceAccountNumber))
            {
                var requestDebito = new RestRequest("api/accounts/movements", Method.Post).AddJsonBody(new
                {
                    requestId = command.IdRequisicao,
                    accountNumber = sourceAccountNumber,
                    valor = command.Valor,
                    tipo = "D"
                });

                var response = await httpClient.ExecuteAsync(requestDebito);

                if (!response.IsSuccessStatusCode)
                    throw new InvalidOperationException("Falha ao debitar da conta de origem");
            }

            // Crédito conta destino
            var requestCredito = new RestRequest("api/accounts/movements", Method.Post).AddJsonBody(new
            {
                requestId = command.IdRequisicao,
                accountNumber = command.NumeroContaDestino,
                valor = command.Valor,
                tipo = "C"
            });

            var creditoResponse = await httpClient.ExecuteAsync(requestCredito);

            // Se não tiver sucesso no crédito de destino, efetua o estorno (crédito na conta origem)
            if (!creditoResponse.IsSuccessStatusCode)
            {
                var requestCreditoOrigem = new RestRequest("api/accounts/movements", Method.Post).AddJsonBody(new
                {
                    requestId = command.IdRequisicao,
                    accountNumber = sourceAccountNumber,
                    valor = command.Valor,
                    tipo = "C"
                });

                await httpClient.ExecuteAsync(requestCreditoOrigem);

                throw new InvalidOperationException("Falha ao creditar conta destino");
            }

            destinationAccountId = creditoResponse?.Headers?.FirstOrDefault(h => h.Name == "AccountId")?.Value != null
                  ? Guid.Parse(creditoResponse.Headers.First(h => h.Name == "AccountId").Value)
                  : throw new InvalidOperationException("Falha ao obter ID da conta corrente de origem");

            // Persistir transferência
            var transferencia = new Transferencia
            {
                IdTransferencia = Guid.NewGuid(),
                IdContaCorrenteOrigem = command.ContaOrigemId,
                IdContaCorrenteDestino = destinationAccountId,
                Valor = command.Valor,
                DataMovimento = DateTime.UtcNow
            };

            await _transferenciaRepository.AddAsync(transferencia);
        }
    }
}
