namespace RosbridgeClientCommon.Interfaces
{
    using System.Threading.Tasks;

    public interface IRosPublisher<TRosMessage> where TRosMessage : class, new()
    {
        /// <summary>
        /// Topic for publishing to
        /// </summary>
        string Topic { get; }
        /// <summary>
        /// Topic's message type
        /// </summary>
        string Type { get; }
        /// <summary>
        /// Advertise to the topic the willingness to publish
        /// </summary>
        /// <returns></returns>
        Task AdvertiseAsync();
        /// <summary>
        /// Advertise to the topic that there is no willingness to publish
        /// </summary>
        /// <returns></returns>
        Task UnadvertiseAsync();
        /// <summary>
        /// Publish a message to the topic
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        Task PublishAsync(TRosMessage message);
    }
}
