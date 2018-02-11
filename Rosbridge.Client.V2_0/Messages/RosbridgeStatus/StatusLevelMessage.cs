namespace Rosbridge.Client.V2_0.Messages.RosbridgeStatus
{
    using Newtonsoft.Json;
    using Rosbridge.Client.V2_0.Constants;

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
