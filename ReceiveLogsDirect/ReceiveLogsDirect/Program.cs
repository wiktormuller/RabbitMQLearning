// See https://aka.ms/new-console-template for more information

using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("Receive Logs Direct!");

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.ExchangeDeclare(exchange: "direct_logs", type: "direct");

    var queueName = channel.QueueDeclare().QueueName;

    if (args.Length < 1)
    {
        Console.Error.WriteLine($"Usage: {Environment.GetCommandLineArgs()[0]} [info] [warning] [error]");
        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
        Environment.ExitCode = 1;
        return;
    }

    foreach (var severity in args)
    {
        channel.QueueBind(queue: queueName, exchange: "direct_logs", routingKey: severity);
    }
    
    Console.WriteLine(" [*] Waiting for messages.");

    var consumer = new EventingBasicConsumer((channel));
    consumer.Received += (model, basicDeliverEventArgs) =>
    {
        var body = basicDeliverEventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        var routingKey = basicDeliverEventArgs.RoutingKey;
        
        Console.WriteLine($" [x] Received {routingKey} : {message}");
    };

    channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);

    Console.ReadKey();
}