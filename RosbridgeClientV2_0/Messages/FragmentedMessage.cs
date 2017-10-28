namespace RosbridgeClientV2_0.Messages
{
    using Newtonsoft.Json;

    /// <summary>
    /// For fragmenting large messages.
    /// </summary>
    public class FragmentedMessage : RosbridgeMessageBase
    {
        /// <summary>
        /// Required for fragmented messages, in order to identify corresponding fragments for the fragmented message
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// A fragment of data that, when combined with other fragments of data, makes up another message
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// The index of the fragment in the message
        /// </summary>
        [JsonProperty("num")]
        public int Number { get; set; }

        /// <summary>
        /// The total number of fragments
        /// </summary>
        [JsonProperty("total")]
        public int Total { get; set; }

        public FragmentedMessage() : base(RosbridgeProtocolConstants.MessageTransformation.FRAGMENT)
        {
        }
    }
}
