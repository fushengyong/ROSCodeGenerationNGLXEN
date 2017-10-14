namespace RosbridgeClientV2_0.Messages
{
    using Enums;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Rosbridge sends status messages to the client relating to the successes and failures of rosbridge protocol commands.
    /// </summary>
    public class SetStatusLevelMessage : RosbridgeMessageBase
    {
        public SetStatusLevelMessage()
        {
            Operation = RosbridgeProtocolConstants.StatusMessage.SET_STATUS_LEVEL;
        }

        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        /// <summary>
        /// Sets the status level to the level specified
        /// </summary>
        [JsonProperty("level")]
        [JsonConverter(typeof(StringEnumConverter))]
        public StatusMessageLevel StatusLevel { get; set; }
    }
}
