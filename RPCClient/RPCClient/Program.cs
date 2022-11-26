// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

Console.WriteLine("RPC Client!");

var rpcClient = new RpcClient();

Console.WriteLine(" [x] Requesting fib(30)");
var response = rpcClient.Call("30");

Console.WriteLine($" [.] Got '{response}'");
rpcClient.Close();

class RpcClient
{
    private readonly IConnection connection;
    private readonly IModel channel;
    private readonly string replyQueueName;
    private readonly EventingBasicConsumer consumer;
    private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
    private readonly IBasicProperties props;

    public RpcClient()
    {
        var factory = new ConnectionFactory() {HostName = "localhost"};

        connection = factory.CreateConnection();
        channel = connection.CreateModel();
        replyQueueName = channel.QueueDeclare().QueueName;
        consumer = new EventingBasicConsumer(channel);

        props = channel.CreateBasicProperties();
        var correlationId = Guid.NewGuid().ToString();
        props.CorrelationId = correlationId;
        props.ReplyTo = replyQueueName;

        consumer.Received += (model, basic) =>
        {
            var body = basic.Body.ToArray();
            var response = Encoding.UTF8.GetString(body);

            if (basic.BasicProperties.CorrelationId == correlationId)
            {
                respQueue.Add(response);
            }
        };

        channel.BasicConsume(consumer: consumer, queue: replyQueueName, autoAck: true);
    }

    public string Call(string message)
    {
        var messageBytes = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish(
            exchange: "",
            routingKey: "rpc_queue",
            basicProperties: props,
            body: messageBytes);

        return respQueue.Take();
    }

    public void Close()
    {
        connection.Close();
    }
}

// Summary:
// We establish a connection and channel and declare an exclusive 'callback' queue for replies.
// We subscribe to the 'callback' queue, so that we can receive RPC responses
// Our Call method makes the actual RPC request
// Here, we first generate a unique CorrelationId number and save it to identify the appropriate response when it arrives.
// Next, we publish the request message, with two properties: ReplyTo and CorrelationId
// At this point we can sit back and wait until the proper response arrives.
// For every response message the client checks if the CorrelationId is the one we are looking for. If so, it saves the response.
// Finally we return the response back to the user.
