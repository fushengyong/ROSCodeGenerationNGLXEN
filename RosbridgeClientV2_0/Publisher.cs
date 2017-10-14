namespace RosbridgeClientV2_0
{
    using Messages;
    using Newtonsoft.Json.Linq;
    using RosbridgeClientCommon.Interfaces;
    using System;
    using System.Threading.Tasks;

    public class Publisher<TRosMessage> : IRosPublisher<TRosMessage> where TRosMessage : class, new()
    {
        private IMessageDispatcher _messageDispatcher;
        private readonly string _uniqueId;

        public string Topic { get; private set; }

        public string Type { get; private set; }

        public Publisher(string topic, IMessageDispatcher messageDispatcher)
        {
            if (null == topic)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (string.IsNullOrEmpty(topic))
            {
                throw new ArgumentException("Argument cannot be empty!", nameof(topic));
            }

            if (null == messageDispatcher)
            {
                throw new ArgumentNullException(nameof(messageDispatcher));
            }

            _messageDispatcher = messageDispatcher;
            _uniqueId = _messageDispatcher.GetUID();
            Topic = topic;
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
            JObject jsonMessage = JObject.FromObject(message);

            return Task.Run(() =>
            {
                _messageDispatcher.SendAsync(new RosPublishMessage()
                {
                    Id = _uniqueId,
                    Topic = Topic,
                    Message = jsonMessage
                });
            });
        }
    }
}
