using KafkaFlow;

namespace BankMore.Account.Worker
{
    public class KafkaFlowHostedService : IHostedService
    {
        private readonly IKafkaBus kafkaBus;

        public KafkaFlowHostedService(IServiceProvider serviceProvider)
        {
            kafkaBus = serviceProvider.CreateKafkaBus();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return kafkaBus.StartAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return kafkaBus.StopAsync();
        }
    }
}
