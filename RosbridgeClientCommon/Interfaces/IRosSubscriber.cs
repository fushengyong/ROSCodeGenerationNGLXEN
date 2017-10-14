namespace RosbridgeClientCommon.Interfaces
{
    using EventArguments;
    using System.Threading.Tasks;

    public delegate void RosMessageReceivedHandler<TRosMessage>(object sender, RosMessageReceivedEventArgs<TRosMessage> args) where TRosMessage : class, new();

    public interface IRosSubscriber<TRosMessage> where TRosMessage : class, new()
    {
        event RosMessageReceivedHandler<TRosMessage> RosMessageReceived;
        string Topic { get; }
        string Type { get; }
        Task SubscribeAsync();
        Task UnsubscribeAsync();
    }
}
