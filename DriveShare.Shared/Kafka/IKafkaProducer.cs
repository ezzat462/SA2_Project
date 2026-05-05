using System.Threading.Tasks;

namespace DriveShare.Shared.Kafka
{
    public interface IKafkaProducer
    {
        Task ProduceAsync<T>(string topic, T message);
    }
}
