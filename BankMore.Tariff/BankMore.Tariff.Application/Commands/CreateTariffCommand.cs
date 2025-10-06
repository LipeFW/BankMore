using MediatR;

namespace BankMore.Tariff.Application.Commands
{
    public class CreateTariffCommand : IRequest
    {
        public Guid IdContaCorrente { get; set; }
        public decimal Valor { get; set; }

        public CreateTariffCommand(Guid idContaCorrente, decimal valor)
        {
            IdContaCorrente = idContaCorrente;
            Valor = valor;
        }
    }
}
