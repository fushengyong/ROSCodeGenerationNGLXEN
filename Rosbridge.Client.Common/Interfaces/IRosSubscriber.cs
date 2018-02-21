namespace Rosbridge.Client.Common.Interfaces
{
    using EventArguments;
    using System.Threading.Tasks;

    public delegate void RosMessageReceivedHandler<TRosMessage>(object sender, RosMessageReceivedEventArgs<TRosMessage> args) where TRosMessage : class, new();

    /// <summary>
    /// ROS topic subscriber
    /// </summary>
    /// <typeparam name="TRosMessage"></typeparam>
    public interface IRosSubscriber<TRosMessage> where TRosMessage : class, new()
    {
        /// <summary>
        /// Message received event
        /// </summary>
        event RosMessageReceivedHandler<TRosMessage> RosMessageReceived;
        /// <summary>
        /// Topic to subscribe to
        /// </summary>
        string Topic { get; }
        /// <summary>
        /// Topic's message type
        /// </summary>
        string Type { get; }
        /// <summary>
        /// Subscribe to topic
        /// </summary>
        /// <returns></returns>
        Task SubscribeAsync();
        /// <summary>
        /// Unsubscribe from topic
        /// </summary>
        /// <returns></returns>
        Task UnsubscribeAsync();
    }
}
