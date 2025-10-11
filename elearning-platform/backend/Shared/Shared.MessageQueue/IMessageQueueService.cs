using System;
using System.Threading.Tasks;

namespace Shared.MessageQueue
{
    /// <summary>
    /// Message queue service interface
    /// </summary>
    public interface IMessageQueueService
    {
        /// <summary>
        /// Publish a message to a queue
        /// </summary>
        Task PublishAsync<T>(string queueName, T message) where T : class;

        /// <summary>
        /// Subscribe to a queue and process messages
        /// </summary>
        void Subscribe<T>(string queueName, Action<T> onMessage) where T : class;

        /// <summary>
        /// Close connection
        /// </summary>
        void Dispose();
    }
}