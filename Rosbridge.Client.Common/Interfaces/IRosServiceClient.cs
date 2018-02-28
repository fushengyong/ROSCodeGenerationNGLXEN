namespace Rosbridge.Client.Common.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// ROS service caller
    /// </summary>
    /// <typeparam name="TServiceRequest"></typeparam>
    /// <typeparam name="TServiceResponse"></typeparam>
    public interface IRosServiceClient<TServiceRequest, TServiceResponse> where TServiceRequest : class, new() where TServiceResponse : class, new()
    {
        /// <summary>
        /// The service to call
        /// </summary>
        string ServiceName { get; }
        /// <summary>
        /// Call the service with a specific request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<TServiceResponse> Call(TServiceRequest request);
    }
}
