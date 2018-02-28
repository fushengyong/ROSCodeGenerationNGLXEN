namespace Rosbridge.Client.V2_0
{
    using Newtonsoft.Json.Linq;
    using Rosbridge.Client.Common.EventArguments;
    using Rosbridge.Client.Common.Interfaces;
    using Rosbridge.Client.V2_0.Messages.RosOperations;
    using System;
    using System.Threading.Tasks;

    public class ServiceClient<TServiceRequest, TServiceResponse> : IRosServiceClient<TServiceRequest, TServiceResponse> where TServiceRequest : class, new() where TServiceResponse : class, new()
    {
        private readonly IMessageDispatcher _messageDispatcher;
        private readonly IMessageSerializer _messageSerializer;
        private readonly string _uniqueId;

        public string ServiceName { get; private set; }

        public ServiceClient(string serviceName, IMessageDispatcher messageDispatcher, IMessageSerializer messageSerializer)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
            {
                throw new ArgumentException("Argument cannot be empty!", nameof(serviceName));
            }

            if (null == messageDispatcher)
            {
                throw new ArgumentNullException(nameof(messageDispatcher));
            }

            if (null == messageSerializer)
            {
                throw new ArgumentNullException(nameof(messageSerializer));
            }

            ServiceName = serviceName;
            _messageDispatcher = messageDispatcher;
            _messageSerializer = messageSerializer;
            _uniqueId = _messageDispatcher.GetNewUniqueID();
        }

        public Task<TServiceResponse> Call(TServiceRequest request = null)
        {
            TaskCompletionSource<TServiceResponse> taskCompletion = new TaskCompletionSource<TServiceResponse>();

            MessageReceivedHandler rosbridgeMessageHandler = (object sender, RosbridgeMessageReceivedEventArgs args) =>
            {
                if (null != args)
                {
                    RosServiceResponseMessage rosServiceResponse = args.RosbridgeMessage.ToObject<RosServiceResponseMessage>();

                    if (null != rosServiceResponse && rosServiceResponse.Id.Equals(_uniqueId))
                    {
                        if (null != rosServiceResponse.ValueList)
                        {
                            JObject responseObject = JObject.FromObject(rosServiceResponse.ValueList);

                            TServiceResponse rosServiceResponseObject =
                                responseObject.ToObject<TServiceResponse>();

                            taskCompletion.SetResult(rosServiceResponseObject);
                        }
                        else
                        {
                            taskCompletion.SetResult(null);
                        }
                    }
                }
            };

            _messageDispatcher.MessageReceived += rosbridgeMessageHandler;

            Task.Run(async () =>
            {
                if (null == request)
                {
                    await _messageDispatcher.SendAsync(new RosCallServiceMessage()
                    {
                        Id = _uniqueId,
                        Service = ServiceName
                    });
                }
                else
                {
                    JArray arguments = JArray.FromObject(request);

                    await _messageDispatcher.SendAsync(new RosCallServiceMessage()
                    {
                        Id = _uniqueId,
                        Service = ServiceName,
                        Arguments = arguments
                    });
                }

                await taskCompletion.Task;

                _messageDispatcher.MessageReceived -= rosbridgeMessageHandler;
            });

            return taskCompletion.Task;
        }
    }
}
