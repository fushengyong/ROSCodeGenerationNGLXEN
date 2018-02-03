namespace RosbridgeClientV2_0.Messages.RosOperations
{
    using RosbridgeClientV2_0.Constants;

    public class RosUnsubscribeMessage : RosTopicMessageBase
    {
        public RosUnsubscribeMessage() : base(RosbridgeProtocolConstants.RosMessages.UNSUBSCRIBE)
        {
        }
    }
}
