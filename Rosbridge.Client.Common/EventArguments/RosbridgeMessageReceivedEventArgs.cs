namespace Rosbridge.Client.Common.EventArguments
{
    using Newtonsoft.Json.Linq;
    using System;

    /// <summary>
    /// Contains the received Rosbridge message in JSON format
    /// </summary>
    public class RosbridgeMessageReceivedEventArgs : EventArgs
    {
        public JObject RosBridgeMessage { get; private set; }

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
