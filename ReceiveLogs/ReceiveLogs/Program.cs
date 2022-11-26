// See https://aka.ms/new-console-template for more information

using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("Hello, World!");

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Fanout);

    var queueName = channel.QueueDeclare().QueueName;
    
    channel.QueueBind(queue: queueName, exchange: "logs", routingKey: "");
    
    Console.WriteLine(" [x] Waiting for logs.");

    var consumer = new EventingBasicConsumer(channel);

    consumer.Received += (model, basicDeliverEventArgs) =>
    {
        var body = basicDeliverEventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        
        Console.WriteLine($" [x] {message}");
    };

    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

    Console.ReadKey();
}