using Microsoft.AspNetCore.Mvc;
using RabbitMQ.Client;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Channels;

namespace DotNetMQ.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RabbitMQTestController : ControllerBase
    {
        private readonly ILogger<RabbitMQTestController> _logger;
        //Name of the queue
        
        //Queue to be used in the controller
        private readonly IModel _rabbitMQueue;
        private readonly IModel _rpcQueue;
        private readonly IConnection _connection;
        public RabbitMQTestController(ILogger<RabbitMQTestController> logger)
        {
            _logger = logger;
            //RabbitMQ sender setup
            var _factory = new ConnectionFactory();
            _connection = _factory.CreateConnection();
            _rabbitMQueue = _connection.CreateModel();

            #region Normal Queue
            _rabbitMQueue.QueueDeclare(queue: "UploadQueue",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
            #endregion

            #region RPC Queue
            _rpcQueue = _connection.CreateModel();
            _rpcQueue.QueueDeclare(queue: "RPCQueue",
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
            _rpcQueue.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            #endregion
        }

        [HttpGet]
        public IActionResult SendNormalQueue(string message)
        {
            _rabbitMQueue.BasicPublish(exchange: string.Empty,
                             routingKey: "UploadQueue",
                             basicProperties: null,
                             body: Encoding.UTF8.GetBytes(message)
                             );
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetRPCQueueMessages(int number)
        {
            using var rpcClient = new RpcClient();
            return Ok(await rpcClient.CallAsync("30"));
            
        }
    }
}