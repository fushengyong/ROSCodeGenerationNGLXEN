namespace RosbridgeClientCommon.EventArguments
{
    using Newtonsoft.Json.Linq;
    using System;

    public class RosbridgeMessageReceivedEventArgs : EventArgs
    {
        public JObject RosBridgeMessage { get; }

        public RosbridgeMessageReceivedEventArgs(JObject message)
        {
            if (null == message)
            {
                throw new ArgumentNullException(nameof(message));
            }

            RosBridgeMessage = message;
        }
    }
}
