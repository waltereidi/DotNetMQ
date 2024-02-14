using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

//Queue parameters 
ConnectionFactory _factory = new ConnectionFactory { HostName = "localhost" };
string _QUEUENAME = "UploadQueue";

#region initialization
using var connection = _factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: _QUEUENAME,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

var consumer = new EventingBasicConsumer(channel);
channel.BasicConsume(queue: _QUEUENAME,
                     autoAck: true,
                     consumer: consumer);
#endregion

#region received messages actions
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(message);

};
#endregion

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();