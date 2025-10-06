using BankMore.Tariff.Application.Commands;
using BankMore.Tariff.Domain.Messages;
using KafkaFlow;
using MediatR;

namespace BankMore.Tariff.Worker.Handlers
{
    public class TransferMessageHandler : IMessageHandler<TransferMessage>
    {
        private readonly IMediator _mediator;

        public TransferMessageHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task Handle(IMessageContext context, TransferMessage message)
        {
            Console.WriteLine($"[Handler] Requisição: {message.IdRequisicao}, Valor: {message.Valor}");

            await _mediator.Send(new CreateTariffCommand(message.IdContaCorrente, message.Valor));

        }
    }
}
