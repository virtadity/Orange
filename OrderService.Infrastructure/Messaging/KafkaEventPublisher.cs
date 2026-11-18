using System.Text.Json;
using Confluent.Kafka;
using OrderService.Domain.Interfaces;

namespace OrderService.Infrastructure.Messaging;

public class KafkaEventPublisher : IEventPublisher, IDisposable
{
    private readonly IProducer<Null, string> _producer;

    public KafkaEventPublisher(string bootstrapServers)
    {
        var config = new ProducerConfig { BootstrapServers = bootstrapServers };
        _producer = new ProducerBuilder<Null, string>(config).Build();
    }

    public async Task PublishAsync<T>(string topic, T message, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(message);
        await _producer.ProduceAsync(topic, new Message<Null, string> { Value = json }, ct);
    }

    public void Dispose() => _producer.Dispose();
}
