using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory _factory = new ConnectionFactory { HostName = "localhost" };
string _QUEUENAME = "UploadQueue";
string _HOSTNAME = "https://localhost:5556";
using var connection = _factory.CreateConnection();
using var channel = connection.CreateModel();

channel.QueueDeclare(queue: _QUEUENAME,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine(message);

};
channel.BasicConsume(queue: _QUEUENAME,
                     autoAck: true,
                     consumer: consumer);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();