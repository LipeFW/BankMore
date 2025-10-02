using BankMore.Transfer.Domain.Entities;
using BankMore.Transfer.Domain.Exceptions;
using BankMore.Transfer.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;

namespace BankMore.Transfer.Application.Commands
{
    public class TransferHandler : IRequestHandler<TransferCommand>
    {
        private readonly HttpClient _httpClient;
        private readonly ITransferRepository  _transferenciaRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TransferHandler(HttpClient httpClient, ITransferRepository transferenciaRepository, IHttpContextAccessor httpContext)
        {
            _httpClient = httpClient;
            _transferenciaRepository = transferenciaRepository;
            _httpContextAccessor = httpContext;
        }

        public async Task Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            if(request.Valor <= 0)
                throw new InvalidValueException("Valor inválido");

            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.Replace("Bearer ", ""));
            }


            // Débito conta origem
            var debitoResponse = await _httpClient.PostAsJsonAsync("/api/accounts/movimento", new
            {
                idRequisicao = request.IdRequisicao,
                valor = request.Valor,
                tipo = "D"
            }, cancellationToken);

            if (!debitoResponse.IsSuccessStatusCode)
                throw new InvalidOperationException("Falha ao debitar conta origem");

            // Crédito conta destino
            var creditoResponse = await _httpClient.PostAsJsonAsync("/api/accounts/movimento", new
            {
                idRequisicao = request.IdRequisicao,
                numeroConta = request.NumeroContaDestino,
                valor = request.Valor,
                tipo = "C"
            }, cancellationToken);

            if (!creditoResponse.IsSuccessStatusCode)
            {
                // Estorno (crédito na conta origem)
                await _httpClient.PostAsJsonAsync("/api/accounts/movimento", new
                {
                    idRequisicao = request.IdRequisicao,
                    valor = request.Valor,
                    tipo = "C"
                }, cancellationToken);

                throw new InvalidOperationException("Falha ao creditar conta destino");
            }

            // Persistir transferência
            var transferencia = new Transferencia
            {
                IdTransferencia = Guid.NewGuid(),
                IdContaCorrenteOrigem = request.ContaOrigemId,
                // você precisa resolver o Id da conta destino via repo (buscar pelo NumeroContaDestino)
                IdContaCorrenteDestino = Guid.NewGuid(),
                Valor = request.Valor,
                DataMovimento = DateTime.UtcNow
            };

            await _transferenciaRepository.AddAsync(transferencia);
        }
    }
}
