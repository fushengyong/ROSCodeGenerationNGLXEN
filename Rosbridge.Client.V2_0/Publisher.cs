namespace Rosbridge.Client.V2_0
{
    using Newtonsoft.Json.Linq;
    using Rosbridge.Client.Common.Interfaces;
    using Rosbridge.Client.V2_0.Messages.RosOperations;
    using System;
    using System.Threading.Tasks;

    public class Publisher<TRosMessage> : IRosPublisher<TRosMessage> where TRosMessage : class, new()
    {
        private readonly IMessageDispatcher _messageDispatcher;
        protected internal readonly string _uniqueId;

        public string Topic { get; private set; }

        public string Type { get; private set; }

        public Publisher(string topic, IMessageDispatcher messageDispatcher, IRosMessageTypeAttributeHelper rosMessageTypeAttributeHelper)
        {
            if (string.IsNullOrWhiteSpace(topic))
            {
                throw new ArgumentException("Argument cannot be empty!", nameof(topic));
            }

            if (null == messageDispatcher)
            {
                throw new ArgumentNullException(nameof(messageDispatcher));
            }

            if (null == rosMessageTypeAttributeHelper)
            {
                throw new ArgumentNullException(nameof(rosMessageTypeAttributeHelper));
            }

            _messageDispatcher = messageDispatcher;
            _uniqueId = _messageDispatcher.GetNewUniqueID();
            Topic = topic;
            Type = rosMessageTypeAttributeHelper.GetRosMessageTypeFromTypeAttribute(typeof(TRosMessage));
        }

        public Task AdvertiseAsync()
        {
            return _messageDispatcher.SendAsync(new RosAdvertiseMessage()
            {
                Id = _uniqueId,
                Topic = Topic,
                Type = Type
            });
        }

        public Task UnadvertiseAsync()
        {
            return _messageDispatcher.SendAsync(new RosUnadvertiseMessage()
            {
                Id = _uniqueId,
                Topic = Topic
            });
        }

        public Task PublishAsync(TRosMessage message)
        {
            if (null == message)
            {
                throw new ArgumentNullException(nameof(message));
            }

            JObject jsonMessage = JObject.FromObject(message);

            return _messageDispatcher.SendAsync(new RosPublishMessage()
            {
                Id = _uniqueId,
                Topic = Topic,
                Message = jsonMessage
            });
        }
    }
}