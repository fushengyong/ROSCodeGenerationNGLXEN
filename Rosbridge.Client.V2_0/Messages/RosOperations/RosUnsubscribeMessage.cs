namespace Rosbridge.Client.V2_0.Messages.RosOperations
{
    using Rosbridge.Client.V2_0.Constants;

    public class RosUnsubscribeMessage : RosTopicMessageBase
    {
        public RosUnsubscribeMessage() : base(RosbridgeProtocolConstants.RosMessages.UNSUBSCRIBE)
        {
        }
    }
}
