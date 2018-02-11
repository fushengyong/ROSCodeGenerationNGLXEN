namespace Rosbridge.Client.V2_0.Messages.MessageTransformation
{
    using Newtonsoft.Json;
    using Rosbridge.Client.V2_0.Constants;

    /// <summary>
    /// Some messages (such as point clouds) can be extremely large, and for efficiency reasons we may wish to transfer them as PNG-encoded bytes. 
    /// </summary>
    public class PNGMessage : RosbridgeMessageBase
    {
        /// <summary>
        /// Only required if the message is fragmented. Identifies the fragments for the fragmented message.
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        /// <summary>
        /// A fragment of data that, when combined with other fragments of data, makes up another message
        /// </summary>
        [JsonProperty("data")]
        public string Data { get; set; }

        /// <summary>
        /// The index of the fragment in the message
        /// </summary>
        [JsonProperty("num", NullValueHandling = NullValueHandling.Ignore)]
        public int? Number { get; set; }

        /// <summary>
        /// The total number of fragments
        /// </summary>
        [JsonProperty("total", NullValueHandling = NullValueHandling.Ignore)]
        public int? Total { get; set; }

        public PNGMessage() : base(RosbridgeProtocolConstants.MessageTransformation.PNG)
        {
        }
    }
}
