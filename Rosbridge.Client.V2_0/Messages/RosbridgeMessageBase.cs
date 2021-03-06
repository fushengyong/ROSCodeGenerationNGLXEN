﻿namespace Rosbridge.Client.V2_0.Messages
{
    using Newtonsoft.Json;

    public abstract class RosbridgeMessageBase
    {
        /// <summary>
        /// Indicates the type of message that this is. Messages with different values for op may be handled differently.
        /// </summary>
        [JsonProperty("op")]
        public string Operation { get; private set; }

        protected RosbridgeMessageBase(string operation)
        {
            Operation = operation;
        }
    }
}
