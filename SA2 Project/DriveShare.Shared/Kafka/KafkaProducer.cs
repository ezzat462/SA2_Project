using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace DriveShare.Shared.Kafka
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IConfiguration _config;

        public KafkaProducer(IConfiguration config)
        {
            _config = config;
        }

        public async Task ProduceAsync<T>(string topic, T message)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = _config["Kafka:BootstrapServers"]
            };

            using var producer = new ProducerBuilder<Null, string>(config).Build();
            var value = JsonSerializer.Serialize(message);

            await producer.ProduceAsync(topic, new Message<Null, string> { Value = value });
        }
    }
}
