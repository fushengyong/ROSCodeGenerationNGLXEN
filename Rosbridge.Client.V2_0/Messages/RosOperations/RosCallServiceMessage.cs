﻿namespace Rosbridge.Client.V2_0.Messages.RosOperations
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;
    using Rosbridge.Client.V2_0.Constants;
    using Rosbridge.Client.V2_0.Enums;

    public class RosCallServiceMessage : RosMessageBase
    {
        /// <summary>
        /// The name of the service to call
        /// </summary>
        [JsonProperty("service")]
        public string Service { get; set; }

        /// <summary>
        /// If the service has no args, then args does not have to be provided, though an empty list is equally acceptable. Args should be a list of json objects representing the arguments to the service
        /// </summary>
        [JsonProperty("args", NullValueHandling = NullValueHandling.Ignore)]
        public JArray Arguments { get; set; }

        /// <summary>
        /// The maximum size that the response message can take before it is fragmented
        /// </summary>
        [JsonProperty("fragment_size", NullValueHandling = NullValueHandling.Ignore)]
        public int? FragmentSize { get; set; }

        /// <summary>
        /// Specifies the compression scheme to be used on messages.
        /// </summary>
        [JsonProperty("compression", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageCompressionLevel? Compression { get; set; }

        public RosCallServiceMessage() : base(RosbridgeProtocolConstants.RosMessages.CALL_SERVICE)
        {
        }
    }
}
