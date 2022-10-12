// See https://aka.ms/new-console-template for more information

using System.Text;
using RabbitMQ.Client;

///
/// Hello World project
/// 
// Console.WriteLine("Sender!");
//
// var factory = new ConnectionFactory() { HostName = "localhost", Password = "guest", UserName = "guest" };
//
// using (var connection = factory.CreateConnection())
// using (var channel = connection.CreateModel())
// {
//     channel.QueueDeclare(queue: "hello", durable: false, autoDelete: false, arguments: null, exclusive: false);
//
//     string message = "Hello World!";
//     var body = Encoding.UTF8.GetBytes(message);
//     
//     channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);
//
//     Console.WriteLine($" [x] Sent {message}");
//     
//     Console.ReadLine();
// }


///
/// Work Queues project
/// 
Console.WriteLine("Sender!");

var factory = new ConnectionFactory() { HostName = "localhost", Password = "guest", UserName = "guest" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "hello", durable: true, autoDelete: false, arguments: null, exclusive: false);

    string message = GetMessage(args);
    var body = Encoding.UTF8.GetBytes(message);

    var properties = channel.CreateBasicProperties();
    properties.Persistent = true;
    
    channel.BasicPublish(exchange: "", routingKey: "hello", basicProperties: null, body: body);

    Console.WriteLine($" [x] Sent {message}");
    
    Console.ReadLine();
}

static string GetMessage(string[] args)
{
    return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
}