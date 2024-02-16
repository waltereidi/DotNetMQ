using System.Text;
using System.Threading.Channels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;


public class RpcQueueServer
{
    private  IModel channel { get; set; }
    private  IBasicProperties replyProps { get; set; }
    private  EventingBasicConsumer consumer { get; set; }

    public RpcQueueServer()
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
        consumer = new EventingBasicConsumer(channel);
        channel.BasicConsume(queue: "RPCQueue",
                             autoAck: false,
                             consumer: consumer);
        #endregion

        #region receive messages to send
        consumer.Received += (model, ea) =>
        {
            string response = string.Empty;

            var body = ea.Body.ToArray();
            var props = ea.BasicProperties;
            replyProps = channel.CreateBasicProperties();
            replyProps.CorrelationId = props.CorrelationId;

            replyProps = channel.CreateBasicProperties();
            replyProps.CorrelationId = ea.BasicProperties.CorrelationId;

            channel.BasicPublish(exchange: string.Empty,
                                    routingKey: ea.BasicProperties.ReplyTo,
                                    basicProperties: replyProps,
                                    body: ea.Body
                                    );
            channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
        };
        #endregion


    }
}   