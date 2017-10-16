namespace RosbridgeClientCommon.Interfaces
{
    using System.Threading.Tasks;

    public interface IRosPublisher<TRosMessage> where TRosMessage : class, new()
    {
        string Topic { get; }
        string Type { get; }
        Task AdvertiseAsync();
        Task UnadvertiseAsync();
        Task PublishAsync(TRosMessage message);
    }
}
