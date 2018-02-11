namespace Rosbridge.Client.V2_0.Messages.RosOperations
{
    using Rosbridge.Client.V2_0.Constants;

    /// <summary>
    /// Use this class to stop advertising that you are publishing to a topic.
    /// </summary>
    public class RosUnadvertiseMessage : RosTopicMessageBase
    {
        public RosUnadvertiseMessage() : base(RosbridgeProtocolConstants.RosMessages.UNADVERTISE)
        {
        }
    }
}
