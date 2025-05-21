using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory()
{
    HostName = "localhost"
};

var connection = await factory.CreateConnectionAsync();
var channel = await connection.CreateChannelAsync();

// Pastikan queue ada
await channel.QueueDeclareAsync(
    queue: "message",
    durable: true,
    exclusive: false,
    autoDelete: false,
    arguments: null
);

var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);

    Console.WriteLine($"[x] Received: {message}");

    await Task.Delay(500);

    await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
};

await channel.BasicConsumeAsync(
    queue: "message",
    autoAck: false, // Manual ack
    consumer: consumer
);

Console.WriteLine(" [*] Waiting for messages. Press [enter] to exit.");
Console.ReadLine();

// await channel.CloseAsync();
// await connection.CloseAsync();