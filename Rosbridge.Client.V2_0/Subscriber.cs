namespace Rosbridge.Client.V2_0
{
    using Rosbridge.Client.Common.EventArguments;
    using Rosbridge.Client.Common.Interfaces;
    using Rosbridge.Client.V2_0.Messages.RosOperations;
    using System;
    using System.Threading.Tasks;

    public class Subscriber<TRosMessage> : IRosSubscriber<TRosMessage> where TRosMessage : class, new()
    {
        private readonly IMessageDispatcher _messageDispatcher;
        protected internal readonly string _uniqueId;

        public event RosMessageReceivedHandler<TRosMessage> RosMessageReceived;

        public string Topic { get; private set; }

        public string Type { get; private set; }

        public Subscriber(string topic, IMessageDispatcher messageDispatcher, IRosMessageTypeAttributeHelper rosMessageTypeAttributeHelper)
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
            _messageDispatcher.MessageReceived += RosbridgeMessageReceived;
            _uniqueId = _messageDispatcher.GetNewUniqueID();
            Topic = topic;
            Type = rosMessageTypeAttributeHelper.GetRosMessageTypeFromTypeAttribute(typeof(TRosMessage));
        }


        public Task SubscribeAsync()
        {
            return _messageDispatcher.SendAsync(new RosSubscribeMessage()
            {
                Id = _uniqueId,
                Topic = Topic,
                Type = Type
            });
        }

        public Task UnsubscribeAsync()
        {
            return _messageDispatcher.SendAsync(new RosUnsubscribeMessage()
            {
                Id = _uniqueId,
                Topic = Topic
            });
        }

        private void RosbridgeMessageReceived(object sender, RosbridgeMessageReceivedEventArgs args)
        {
            if (null != args)
            {
                RosPublishMessage receivedPublishMessage = args.RosbridgeMessage.ToObject<RosPublishMessage>();

                if (null != RosMessageReceived && null != receivedPublishMessage && !string.IsNullOrEmpty(receivedPublishMessage.Topic) && receivedPublishMessage.Topic.Equals(this.Topic))
                {
                    TRosMessage receivedRosMessage = receivedPublishMessage.Message.ToObject<TRosMessage>();

                    RosMessageReceived(this, new RosMessageReceivedEventArgs<TRosMessage>(receivedRosMessage));
                }
            }
        }
    }
}
