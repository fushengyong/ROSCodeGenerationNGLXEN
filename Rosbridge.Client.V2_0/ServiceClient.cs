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
        private IMessageDispatcher _messageDispatcher;
        private readonly string _uniqueId;

        public string Service { get; private set; }

        public ServiceClient(string service, IMessageDispatcher messageDispatcher)
        {
            if (null == service)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (string.IsNullOrWhiteSpace(service))
            {
                throw new ArgumentException("Argument cannot be empty!", nameof(service));
            }

            if (null == messageDispatcher)
            {
                throw new ArgumentNullException(nameof(messageDispatcher));
            }

            _messageDispatcher = messageDispatcher;
            _uniqueId = _messageDispatcher.GetNewUniqueID();
        }

        public Task<TServiceResponse> Call(TServiceRequest request)
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
