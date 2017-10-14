namespace RosbridgeClientCommon.EventArguments
{
    using System;

    public class RosMessageReceivedEventArgs<TRosMessage> : EventArgs where TRosMessage : class, new()
    {
        public TRosMessage RosMessage;

        public RosMessageReceivedEventArgs(TRosMessage message)
        {
            RosMessage = message;
        }
    }
}
