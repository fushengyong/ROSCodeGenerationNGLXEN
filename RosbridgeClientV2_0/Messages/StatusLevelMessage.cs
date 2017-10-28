namespace RosbridgeClientV2_0.Messages
{
    using Newtonsoft.Json;

    public class StatusLevelMessage : SetStatusLevelMessage
    {
        /// <summary>
        /// The string message being logged
        /// </summary>
        [JsonProperty("msg")]
        public string Message { get; set; }

        public StatusLevelMessage() : base(RosbridgeProtocolConstants.StatusMessage.STATUS)
        {
        }
    }
}
