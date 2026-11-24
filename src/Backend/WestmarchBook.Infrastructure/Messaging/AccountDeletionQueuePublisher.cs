using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using WestmarchBook.Domain.Messaging;

namespace WestmarchBook.Infrastructure.Messaging;

public class AccountDeletionQueuePublisher : IScheduleAccountDeletion
{
    private const string QueueName = "account-deletion";
    private readonly IConnection _connection;

    public AccountDeletionQueuePublisher(IConnection connection)
    {
        _connection = connection;
    }

    public async Task Schedule(long userId)
    {
        using var channel = await _connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
            );

        var payload = JsonSerializer.Serialize(new { UserId = userId, ScheduledAt = DateTime.UtcNow });
        var body = Encoding.UTF8.GetBytes(payload);

        var properties = new BasicProperties
        {
            Persistent = true
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: QueueName,
            mandatory: false,
            basicProperties: properties,
            body: body
        );
    }
}
