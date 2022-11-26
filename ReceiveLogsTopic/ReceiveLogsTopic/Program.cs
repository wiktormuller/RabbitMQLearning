// See https://aka.ms/new-console-template for more information

using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("Receive Logs Topic!");

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");

    var queueName = channel.QueueDeclare().QueueName;

    if (args.Length < 1)
    {
        Console.Error.WriteLine("Usage: {0} [binding_key...]",
            Environment.GetCommandLineArgs()[0]);

        Console.ReadKey();
    }

    foreach (var bindingKey in args)
    {
        channel.QueueBind(queue: queueName, exchange: "topic_logs", routingKey: bindingKey);
    }

    Console.WriteLine(" [*] Waiting for messages.");

    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, basicDeliverEventArgs) =>
    {
        var body = basicDeliverEventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var routingKey = basicDeliverEventArgs.RoutingKey;

        Console.WriteLine($"$ [x] Received {routingKey} : {message}");
    };

    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

    Console.ReadKey();
}

// Examples to run:
// "#"
// "kern.*"
// "*.critical"
// "kern.*" "*critical"
