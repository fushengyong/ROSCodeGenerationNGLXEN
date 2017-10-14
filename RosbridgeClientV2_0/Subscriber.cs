﻿namespace RosbridgeClientV2_0
{
    using Messages;
    using RosbridgeClientCommon.EventArguments;
    using RosbridgeClientCommon.Interfaces;
    using System;
    using System.Threading.Tasks;

    public class Subscriber<TRosMessage> : IRosSubscriber<TRosMessage> where TRosMessage : class, new()
    {
        private IMessageDispatcher _messageDispatcher;
        private readonly string _uniqueId;

        public event RosMessageReceivedHandler<TRosMessage> RosMessageReceived;

        public string Topic { get; private set; }

        public string Type { get; private set; }

        public Subscriber(string topic, IMessageDispatcher messageDispatcher)
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
            _messageDispatcher.MessageReceived += RosbridgeMessageReceived;
            _uniqueId = _messageDispatcher.GetUID();
            Topic = topic;
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
                RosPublishMessage receivedPublishMessage = args.RosBridgeMessage.ToObject<RosPublishMessage>();

                if (null != RosMessageReceived && null != receivedPublishMessage && !string.IsNullOrEmpty(receivedPublishMessage.Topic) && receivedPublishMessage.Topic.Equals(Topic))
                {
                    TRosMessage receivedRosMessage = receivedPublishMessage.Message.ToObject<TRosMessage>();
                    RosMessageReceived(this, new RosMessageReceivedEventArgs<TRosMessage>(receivedRosMessage));
                }
            }
        }
    }
}
