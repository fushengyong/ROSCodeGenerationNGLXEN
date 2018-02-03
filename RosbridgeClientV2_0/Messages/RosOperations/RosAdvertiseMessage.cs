namespace RosbridgeClientV2_0.Messages.RosOperations
{
    using Newtonsoft.Json;
    using RosbridgeClientV2_0.Constants;

    /// <summary>
    /// Use this class to advertise that you are or will be publishing a topic.
    /// </summary>
    public class RosAdvertiseMessage : RosTopicMessageBase
    {
        /// <summary>
        /// The string type to advertise for the topic
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        public RosAdvertiseMessage() : base(RosbridgeProtocolConstants.RosMessages.ADVERTISE)
        {
        }
    }
}
