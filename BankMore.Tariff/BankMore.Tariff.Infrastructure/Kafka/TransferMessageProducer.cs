using BankMore.Tariff.Domain.Interfaces;
using BankMore.Tariff.Domain.Messages;
using KafkaFlow.Producers;

namespace BankMore.Tariff.Infrastructure.Kafka
{
    public class TariffMessageProducer : ITariffMessageProducer
    {
        private readonly IProducerAccessor _producerAccessor;

        public TariffMessageProducer(IProducerAccessor producerAccessor)
        {
            _producerAccessor = producerAccessor;
        }

        public async Task PublishAsync(TariffMessage message)
        {
            try
            {
                var producer = _producerAccessor.GetProducer("tariff-producer");

                await producer.ProduceAsync(Guid.NewGuid().ToString(), message);


                Console.WriteLine($"Mensagem Tariff enviada para Account: {message.IdContaCorrente}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar mensagem para Account: {ex}");
            }
        }
    }
}
