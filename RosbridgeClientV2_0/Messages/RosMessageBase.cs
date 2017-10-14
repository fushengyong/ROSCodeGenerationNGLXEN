namespace RosbridgeClientV2_0.Messages
{
    using Newtonsoft.Json;

    public abstract class RosMessageBase : RosbridgeMessageBase
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }
    }
}
