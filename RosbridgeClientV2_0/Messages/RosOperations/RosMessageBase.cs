namespace RosbridgeClientV2_0.Messages.RosOperations
{
    using Newtonsoft.Json;

    public abstract class RosMessageBase : RosbridgeMessageBase
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        protected RosMessageBase(string operation) : base(operation)
        {
        }
    }
}
