using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

//Queue parameters
var factory = new ConnectionFactory { HostName = "localhost" };
string QUEUENAME = "RPCQueue";

#region initialization
using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

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
consumer.Received += (model, ea) =>
{
    if (ea.BasicProperties.ReplyTo == null)
        return;
    //Utilized params from request
    //ea.BasicProperties.CorrelationId;
    //ea.BasicProperties.ReplyTo
    //ea.Body.ToString() this is the received message content from the api basic publishing
    //Create the basic props to setup the send to the client

    var replyProps = channel.CreateBasicProperties();
    replyProps.CorrelationId = ea.BasicProperties.CorrelationId;
    
    channel.BasicPublish(exchange: string.Empty,
                            routingKey: ea.BasicProperties.ReplyTo,
                            basicProperties: replyProps,
                            body: ea.Body
                            );
    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
};
#endregion
Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();
