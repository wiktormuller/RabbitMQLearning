// See https://aka.ms/new-console-template for more information

using System.Reflection.Emit;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("RPC Server!");

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare(queue: "rpc_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);
    
    channel.BasicQos(0, 1, false);

    var consumer = new EventingBasicConsumer(channel);
    channel.BasicConsume(queue: "rpc_queue", autoAck: false, consumer: consumer);
    
    Console.WriteLine(" [x] Awaiting RPC requests.");

    consumer.Received += (model, basicDeliverEventArgs) =>
    {
        string response = null;

        var body = basicDeliverEventArgs.Body.ToArray();
        var props = basicDeliverEventArgs.BasicProperties;
        var replyProps = channel.CreateBasicProperties();
        replyProps.CorrelationId = props.CorrelationId;

        try
        {
            var message = Encoding.UTF8.GetString(body);
            int n = int.Parse(message);

            Console.WriteLine($" [.] fib({message})");
            response = fib(n).ToString();
        }
        catch (Exception e)
        {
            Console.WriteLine(" [.] " + e.Message);
            response = "";
        }
        finally
        {
            var responseBytes = Encoding.UTF8.GetBytes(response);
            channel.BasicPublish(exchange: "", routingKey: props.ReplyTo, basicProperties: replyProps, body: responseBytes);
            channel.BasicAck(deliveryTag: basicDeliverEventArgs.DeliveryTag, multiple: false);
        }
    };

    Console.ReadKey();
}

static int fib(int n)
{
    if (n == 0 || n == 1)
    {
        return n;
    }

    return fib(n - 1) + fib(n - 2);
}

// As usual we start by establishing the connection, channel and declaring the queue
// We might want to run more than one server process. In order to spread the load equally over multiple servers we need to set the prefetchCount setting in channel.BasicQos.
// We use BasicConsume to access the queue. Then we register a delivery handler in which we do the work and send the response back.