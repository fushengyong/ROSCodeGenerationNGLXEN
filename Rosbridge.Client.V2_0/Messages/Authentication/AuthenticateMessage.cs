namespace Rosbridge.Client.V2_0.Messages.Authentication
{
    using Newtonsoft.Json;
    using Rosbridge.Client.V2_0.Constants;

    /// <summary>
    /// Optional authentication information can be passed via the rosbridge protocol to authenticate a client connection
    /// </summary>
    public class AuthenticateMessage : RosbridgeMessageBase
    {
        /// <summary>
        /// MAC (hashed) string given by the client
        /// </summary>
        [JsonProperty("mac")]
        public string MacAddress { get; set; }

        /// <summary>
        /// IP of the client
        /// </summary>
        [JsonProperty("client")]
        public string ClientIPAddress { get; set; }

        /// <summary>
        /// IP of the destination
        /// </summary>
        [JsonProperty("dest")]
        public string DestinationIPAddress { get; set; }

        /// <summary>
        /// Random string given by the client
        /// </summary>
        [JsonProperty("rand")]
        public string Random { get; set; }

        /// <summary>
        /// User level as a string given by the client
        /// </summary>
        [JsonProperty("level")]
        public string UserLevel { get; set; }

        /// <summary>
        /// Time of the authorization request
        /// </summary>
        [JsonProperty("t")]
        public int AuthorizaionTime { get; set; }

        /// <summary>
        /// End time of the client's session
        /// </summary>
        [JsonProperty("end")]
        public int ClientEndTime { get; set; }

        public AuthenticateMessage() : base(RosbridgeProtocolConstants.Authentication.AUTHENTICATE)
        {
        }
    }
}
