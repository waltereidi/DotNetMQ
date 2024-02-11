using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Channels;

namespace DotNetMQ.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RabbitMQTestController : ControllerBase
    {
        private readonly ILogger<RabbitMQTestController> _logger;
        //Name of the queue
        private readonly string _QUEUENAME = "UploadQueue";
        //Queue to be used in the controller
        private readonly IModel _rabbitMQueue;
        private readonly IConnection _connection;
        public RabbitMQTestController(ILogger<RabbitMQTestController> logger)
        {
            _logger = logger;
            //RabbitMQ sender setup
            var _factory = new ConnectionFactory();
            _connection = _factory.CreateConnection();
            _rabbitMQueue = _connection.CreateModel();
            //Setup to send requests 
            _rabbitMQueue.QueueDeclare(queue: _QUEUENAME,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);

            
        }

        [HttpGet(Name = "SendHttpRequest")]
        public IActionResult Get(string message)
        {
            _rabbitMQueue.BasicPublish(exchange: string.Empty,
                             routingKey: _QUEUENAME,
                             basicProperties: null,
                             body: Encoding.UTF8.GetBytes(message)
                             );

            return Ok();
        }
    }
}