namespace RosbridgeClientV2_0.Messages
{
    public class RosUnsubscribeMessage : RosTopicMessageBase
    {
        public RosUnsubscribeMessage() : base(RosbridgeProtocolConstants.RosMessages.UNSUBSCRIBE)
        {
        }
    }
}
