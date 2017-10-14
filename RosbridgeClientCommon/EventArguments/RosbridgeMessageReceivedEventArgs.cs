namespace RosbridgeClientCommon.EventArguments
{
    using Newtonsoft.Json.Linq;
    using System;

    public class RosbridgeMessageReceivedEventArgs : EventArgs
    {
        public JObject RosBridgeMessage { get; }

        public RosbridgeMessageReceivedEventArgs(JObject message)
        {
            RosBridgeMessage = message;
        }
    }
}
