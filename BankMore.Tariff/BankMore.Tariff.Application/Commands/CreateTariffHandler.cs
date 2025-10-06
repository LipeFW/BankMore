using BankMore.Tariff.Domain.Entities;
using BankMore.Tariff.Domain.Interfaces;
using BankMore.Tariff.Domain.Messages;
using MediatR;
using Microsoft.Extensions.Configuration;

namespace BankMore.Tariff.Application.Commands
{
    public class CreateTariffHandler : IRequestHandler<CreateTariffCommand>
    {
        private readonly ITariffRepository _tariffRepository;
        private readonly IConfiguration _configuration;
        private readonly ITariffMessageProducer _tariffProducer;

        public CreateTariffHandler(ITariffRepository tariffRepository,
            IConfiguration configuration,
            ITariffMessageProducer tariffProducer)
        {
            _tariffRepository = tariffRepository;
            _configuration = configuration;
            _tariffProducer = tariffProducer;
        }

        public async Task Handle(CreateTariffCommand request, CancellationToken cancellationToken)
        {
            var valorTarifa = decimal.Parse(_configuration["Tariff:Value"] ?? "2");

            var tarifa = new Tarifa(request.IdContaCorrente, valorTarifa, DateTime.UtcNow);

            await _tariffRepository.AddAsync(tarifa);

            await _tariffProducer.PublishAsync(new TariffMessage(request.IdContaCorrente, valorTarifa));
        }
    }
}
