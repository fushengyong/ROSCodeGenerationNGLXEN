namespace RosbridgeClientV2_0.Messages
{
    using Enums;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Subscribes the client to the specified topic
    /// </summary>
    public class RosSubscribeMessage : RosTopicMessageBase
    {
        public RosSubscribeMessage()
        {
            Operation = RosbridgeProtocolConstants.RosMessages.SUBSCRIBE;
        }

        /// <summary>
        /// The (expected) type of the topic to subscribe to. If left off, type will be inferred, and if the topic doesn't exist then the command to subscribe will fail
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public string Type { get; set; }

        /// <summary>
        /// The minimum amount of time (in ms) that must elapse between messages being sent. Defaults to 0
        /// </summary>
        [JsonProperty("throttle_rate", NullValueHandling = NullValueHandling.Ignore)]
        public int? ThrottleRate { get; set; }

        /// <summary>
        /// The size of the queue to buffer messages. Messages are buffered as a result of the throttle_rate. Defaults to 1.
        /// </summary>
        [JsonProperty("queue_length", NullValueHandling = NullValueHandling.Ignore)]
        public int? QueueLength { get; set; }

        /// <summary>
        /// The maximum size that a message can take before it is to be fragmented.
        /// </summary>
        [JsonProperty("fragment_size", NullValueHandling = NullValueHandling.Ignore)]
        public int? FragmentSize { get; set; }

        /// <summary>
        /// Specifies the compression scheme to be used on messages
        /// </summary>
        [JsonProperty("compression", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageCompressionLevel? Compression { get; set; }
    }
}
