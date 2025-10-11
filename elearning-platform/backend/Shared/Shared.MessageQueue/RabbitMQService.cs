using System;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Shared.MessageQueue
{
    /// <summary>
    /// RabbitMQ implementation of message queue service
    /// </summary>
    public class RabbitMQService : IMessageQueueService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMQService(string hostname, string username, string password, int port = 5672)
        {
            var factory = new ConnectionFactory
            {
                HostName = hostname,
                UserName = username,
                Password = password,
                Port = port,
                RequestedHeartbeat = TimeSpan.FromSeconds(60),
                AutomaticRecoveryEnabled = true,
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        /// <summary>
        /// Publish a message to a queue
        /// </summary>
        public Task PublishAsync<T>(string queueName, T message) where T : class
        {
            try
            {
                // Declare queue
                _channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                // Serialize message
                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                // Set properties
                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.ContentType = "application/json";

                // Publish message
                _channel.BasicPublish(
                    exchange: "",
                    routingKey: queueName,
                    basicProperties: properties,
                    body: body
                );

                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to publish message to queue '{queueName}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Subscribe to a queue and process messages
        /// </summary>
        public void Subscribe<T>(string queueName, Action<T> onMessage) where T : class
        {
            try
            {
                // Declare queue
                _channel.QueueDeclare(
                    queue: queueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                // Set QoS
                _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                // Create consumer
                var consumer = new EventingBasicConsumer(_channel);

                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var body = ea.Body.ToArray();
                        var json = Encoding.UTF8.GetString(body);
                        var message = JsonConvert.DeserializeObject<T>(json);

                        // Process message
                        onMessage(message);

                        // Acknowledge message
                        _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                    catch (Exception ex)
                    {
                        // Log error and reject message
                        Console.WriteLine($"Error processing message: {ex.Message}");
                        _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
                    }
                };

                // Start consuming
                _channel.BasicConsume(
                    queue: queueName,
                    autoAck: false,
                    consumer: consumer
                );
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to subscribe to queue '{queueName}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }
    }
}