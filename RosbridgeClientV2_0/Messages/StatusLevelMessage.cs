namespace RosbridgeClientV2_0.Messages
{
    using Newtonsoft.Json;

    public class StatusLevelMessage : SetStatusLevelMessage
    {
        public StatusLevelMessage()
        {
            Operation = RosbridgeProtocolConstants.StatusMessage.STATUS;
        }

        /// <summary>
        /// The string message being logged
        /// </summary>
        [JsonProperty("msg")]
        public string Message { get; set; }
    }
}
