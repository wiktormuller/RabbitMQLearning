// See https://aka.ms/new-console-template for more information

using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("Second Receiver!");

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "hello", durable: true, autoDelete: false, arguments: null, exclusive: false);
    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
    
    Console.WriteLine(" [x] Waiting for messages.");
    
    var consumer = new EventingBasicConsumer(channel);
    consumer.Received += (model, deliveryEventArgs) =>
    {
        var body = deliveryEventArgs.Body.ToArray();
        var message = Encoding.UTF8.GetString(body);
        Console.WriteLine($" [x] Received {message}");

        // Simulation time consuming task
        int dots = message.Split(".").Length - 1;
        Thread.Sleep(dots * 1000);
        
        Console.WriteLine(" [x] Done");
        
        // Note: it is possible to access the channel via
        // ((EventingBasicConsumer)sender).Model here
        channel.BasicAck(deliveryTag: deliveryEventArgs.DeliveryTag, multiple: false);
    };

    channel.BasicConsume(queue: "hello", autoAck: false, consumer: consumer);
    
    Console.ReadLine();
}