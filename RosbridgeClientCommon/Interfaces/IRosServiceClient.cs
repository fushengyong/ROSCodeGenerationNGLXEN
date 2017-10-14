using System.Threading.Tasks;

namespace RosbridgeClientCommon.Interfaces
{
    public interface IRosServiceClient<TServiceRequest> where TServiceRequest : class, new()
    {
        string Service { get; }
        Task<TServiceResponse> Call<TServiceResponse>(TServiceRequest request) where TServiceResponse : class, new();
    }
}
