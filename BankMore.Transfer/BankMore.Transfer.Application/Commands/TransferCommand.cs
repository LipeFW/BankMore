using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankMore.Transfer.Application.Commands
{
    public class TransferCommand : IRequest
    {
        public Guid IdRequisicao { get; }
        public string NumeroContaDestino { get; }
        public decimal Valor { get; }
        public Guid ContaOrigemId { get; }

        public TransferCommand(Guid idRequisicao, string numeroContaDestino, decimal valor, Guid contaOrigemId)
        {
            IdRequisicao = idRequisicao;
            NumeroContaDestino = numeroContaDestino;
            Valor = valor;
            ContaOrigemId = contaOrigemId;
        }
    }
}
