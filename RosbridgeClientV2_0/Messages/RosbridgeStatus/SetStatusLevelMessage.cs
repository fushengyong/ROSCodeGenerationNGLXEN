namespace RosbridgeClientV2_0.Messages.RosbridgeStatus
{
    using Enums;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using RosbridgeClientV2_0.Constants;

    /// <summary>
    /// Rosbridge sends status messages to the client relating to the successes and failures of rosbridge protocol commands.
    /// </summary>
    public class SetStatusLevelMessage : RosbridgeMessageBase
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        /// <summary>
        /// Sets the status level to the level specified
        /// </summary>
        [JsonProperty("level")]
        [JsonConverter(typeof(StringEnumConverter))]
        public StatusMessageLevel StatusLevel { get; set; }

        public SetStatusLevelMessage() : base(RosbridgeProtocolConstants.StatusMessage.SET_STATUS_LEVEL)
        {
        }

        protected SetStatusLevelMessage(string operation) : base(operation)
        {
        }
    }
}
