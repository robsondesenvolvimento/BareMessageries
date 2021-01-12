using Bare.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Bare.Services.Messageries
{
    public class RabbitService
    {
        private ILogger<RabbitService> _logger;
        private IConfiguration _configuration;
        private string _hostRabbit;
        private string _queue_text;

        public string MsgConsume { get; private set; }

        public RabbitService(ILogger<RabbitService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;

            _hostRabbit = _configuration.GetSection("RabbitHost").Value;
            _queue_text = _configuration.GetSection("RabbitQueue").Value;

            
        }

        public string SendMessage(string message)
        {
            var factory = new ConnectionFactory() { HostName = _hostRabbit };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                

                channel.QueueDeclare(queue: _queue_text,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var messageObject = new Message
                {
                    MessageText = message,
                    ServiceId = "Identificador",
                    TimeStamp = DateTime.Now
                };

                var messageJson = JsonSerializer.Serialize<Message>(messageObject);

                var body = Encoding.UTF8.GetBytes(messageJson);

                channel.BasicPublish(exchange: "",
                                     routingKey: _queue_text,
                                     basicProperties: null,
                                     body: body);

                _logger.LogInformation($"Publish: {messageJson}");

                return messageJson;
            }
        }

        public string ReceiveMessage()
        {

            var tokenSource = new CancellationTokenSource();

            var factory = new ConnectionFactory() { HostName = _hostRabbit };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                var response = channel.QueueDeclare(queue: _queue_text,
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var messageObject = JsonSerializer.Deserialize<Message>(message);

                    _logger.LogInformation($"Consume: {messageObject.MessageText}");

                    MsgConsume = message;

                    tokenSource.Cancel();
                };

                channel.BasicConsume(queue: _queue_text,
                                    autoAck: true,
                                    consumer: consumer);

                while (!tokenSource.Token.IsCancellationRequested)
                {
                }

                return MsgConsume;
            }
        }
    }
}
