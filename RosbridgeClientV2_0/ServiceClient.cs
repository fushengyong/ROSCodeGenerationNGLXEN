namespace RosbridgeClientV2_0
{
    using Messages;
    using Newtonsoft.Json.Linq;
    using RosbridgeClientCommon.EventArguments;
    using RosbridgeClientCommon.Interfaces;
    using System;
    using System.Threading.Tasks;

    public class ServiceClient<TServiceRequest> : IRosServiceClient<TServiceRequest> where TServiceRequest : class, new()
    {
        private IMessageDispatcher _messageDispatcher;
        private readonly string _uniqueId;

        public string Service { get; private set; }

        public ServiceClient(IMessageDispatcher messageDispatcher)
        {
            if (null == messageDispatcher)
            {
                throw new ArgumentNullException(nameof(messageDispatcher));
            }

            _messageDispatcher = messageDispatcher;
            _uniqueId = _messageDispatcher.GetUID();
        }

        public Task<TServiceResponse> Call<TServiceResponse>(TServiceRequest request) where TServiceResponse : class, new()
        {
            if (null == request)
            {
                throw new ArgumentNullException(nameof(request));
            }

            TaskCompletionSource<TServiceResponse> taskCompletion = new TaskCompletionSource<TServiceResponse>();

            MessageReceivedHandler rosbridgeMessageHandler = (object sender, RosbridgeMessageReceivedEventArgs args) =>
            {
                if (null != args)
                {
                    RosServiceResponseMessage rosbridgeServiceResponse = args.RosBridgeMessage.ToObject<RosServiceResponseMessage>();

                    if (null != rosbridgeServiceResponse && rosbridgeServiceResponse.Id.Equals(_uniqueId))
                    {
                        if (null != rosbridgeServiceResponse.ValueList)
                        {
                            JObject responseObject = new JObject(rosbridgeServiceResponse.ValueList);
                            TServiceResponse rosServiceResponse = responseObject.ToObject<TServiceResponse>();

                            taskCompletion.SetResult(rosServiceResponse);
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
                        Service = Service
                    });
                }
                else
                {
                    JArray arguments = new JArray(request);

                    await _messageDispatcher.SendAsync(new RosCallServiceMessage()
                    {
                        Id = _uniqueId,
                        Service = Service,
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
