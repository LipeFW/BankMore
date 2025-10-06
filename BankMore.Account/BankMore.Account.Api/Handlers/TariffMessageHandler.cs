using BankMore.Account.Application.Commands;
using BankMore.Account.Domain.Interfaces;
using BankMore.Account.Domain.Messages;
using KafkaFlow;
using MediatR;

namespace BankMore.Account.Api.Handlers
{
    public class TariffMessageHandler : IMessageHandler<TariffMessage>
    {
        private readonly IMediator _mediator;
        private readonly IAccountRepository _accountRepository;

        public TariffMessageHandler(IMediator mediator, IAccountRepository accountRepository)
        {
            _mediator = mediator;
            _accountRepository = accountRepository;
        }

        public async Task Handle(IMessageContext context, TariffMessage message)
        {
            Console.WriteLine($"[Handler] Conta corrente: {message.IdContaCorrente}, Valor: {message.Valor}");

            var contaCorrente = await _accountRepository.GetByIdAsync(message.IdContaCorrente);

            await _mediator.Send(new MovementAccountCommand(Guid.NewGuid(), 
                contaCorrente.Numero, 
                contaCorrente.IdContaCorrente.ToString("d"), 
                message.Valor, 
                "D"));
        }
    }
}
