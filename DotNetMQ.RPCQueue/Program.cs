using System.Text;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


public static class RpcQueueServer
{
    private static IModel channel { get; set; }
    private static IBasicProperties replyProps { get; set; }
    private static EventingBasicConsumer consumer { get; set; }

    public static void SendRpcQueue(string message)
    {
        consumer.Received += (model, ea) =>
        {
            //Utilized params from request
            //ea.BasicProperties.CorrelationId;
            //ea.BasicProperties.ReplyTo
            //ea.Body.ToString() this is the received message content from the api basic publishing
            //Create the basic props to setup the send to the client

            replyProps = channel.CreateBasicProperties();
            replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

            channel.BasicPublish(exchange: string.Empty,
                                    routingKey: ea.BasicProperties.ReplyTo,
                                    basicProperties: replyProps,
                                    body: Encoding.UTF8.GetBytes(message)
                                    );
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };

    }
    internal static void Main(string[] args)
    {
        //Queue parameters
        var factory = new ConnectionFactory { HostName = "localhost" };
        string QUEUENAME = "RPCQueue";

        #region initialization
        using var connection = factory.CreateConnection();
        channel = connection.CreateModel();

        channel.QueueDeclare(queue: "RPCQueue",
                             durable: false,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
        var consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(queue: "RPCQueue",
                             autoAck: false,
                             consumer: consumer);
        #endregion

        #region receive messages to send
        
        #endregion

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();

    }
}