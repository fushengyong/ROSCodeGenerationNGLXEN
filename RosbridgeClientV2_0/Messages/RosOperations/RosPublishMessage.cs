namespace RosbridgeClientV2_0.Messages.RosOperations
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using RosbridgeClientV2_0.Constants;

    /// <summary>
    /// The publish message is used to send data on a topic.
    /// </summary>
    public class RosPublishMessage : RosTopicMessageBase
    {
        /// <summary>
        /// The message to publish on the topic
        /// </summary>
        [JsonProperty("msg")]
        public JObject Message { get; set; }

        public RosPublishMessage() : base(RosbridgeProtocolConstants.RosMessages.PUBLISH)
        {
        }
    }
}
