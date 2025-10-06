using BankMore.Transfer.Domain.DTOs.Events;
using BankMore.Transfer.Domain.Interfaces;
using KafkaFlow.Producers;

namespace BankMore.Transfer.Infrastructure.Kafka
{
    public class TransferMessageProducer : ITransferMessageProducer
    {
        private readonly IProducerAccessor _producerAccessor;

        public TransferMessageProducer(IProducerAccessor producerAccessor)
        {
            _producerAccessor = producerAccessor;
        }

        public async Task PublishAsync(TransferMessage message)
        {
            try
            {
                var producer = _producerAccessor.GetProducer("transfer-producer");

                await producer.ProduceAsync(Guid.NewGuid().ToString(), message);


                Console.WriteLine($"Mensagem Transfer enviada para Tariff: {message.IdContaCorrente}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao enviar mensagem para Tariff: {ex}");
            }
        }
    }
}
