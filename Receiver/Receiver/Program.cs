// See https://aka.ms/new-console-template for more information

using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("Receiver!");

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "hello", durable: false, autoDelete: false, arguments: null, exclusive: false);

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, deliveryEventArgs) =>
    {
        var body = deliveryEventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($" [x] Received {message}");
    };

    channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);
    
    Console.ReadLine();
}