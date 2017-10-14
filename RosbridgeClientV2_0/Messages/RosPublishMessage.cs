namespace RosbridgeClientV2_0.Messages
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// The publish message is used to send data on a topic.
    /// </summary>
    public class RosPublishMessage : RosTopicMessageBase
    {
        public RosPublishMessage()
        {
            Operation = RosbridgeProtocolConstants.RosMessages.PUBLISH;
        }

        /// <summary>
        /// The message to publish on the topic
        /// </summary>
        [JsonProperty("msg")]
        public JObject Message { get; set; }
    }
}
