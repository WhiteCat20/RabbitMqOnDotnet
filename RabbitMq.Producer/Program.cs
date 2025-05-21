using System.Text;
using RabbitMQ.Client;

var factory = new ConnectionFactory()
{
    HostName = "localhost"
};

await using var connection = await factory.CreateConnectionAsync();
await using var channel = await connection.CreateChannelAsync();

await channel.QueueDeclareAsync(
    queue: "message",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null
);


for (int i = 0; i < 100; i++)
{
    var message = $"{DateTime.UtcNow} - {Guid.NewGuid()}";
    var body = Encoding.UTF8.GetBytes(message);

    await channel.BasicPublishAsync(
        exchange: "",             
        routingKey: "message",
        mandatory: true,
        basicProperties: new BasicProperties()
        {
            Persistent = true,
        },
        body: body
    );

    Console.WriteLine($"Sent: {message}");
    await Task.Delay(2000);
}