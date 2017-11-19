namespace RosbridgeClientCommon.Interfaces
{
    using System.Threading.Tasks;

    public interface IRosServiceClient<TServiceRequest, TServiceResponse> where TServiceRequest : class, new() where TServiceResponse : class, new()
    {
        string Service { get; }
        Task<TServiceResponse> Call(TServiceRequest request);
    }
}
