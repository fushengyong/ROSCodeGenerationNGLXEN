namespace Rosbridge.Client.Common.EventArguments
{
    using System;

    public class RosMessageReceivedEventArgs<TRosMessage> : EventArgs where TRosMessage : class, new()
    {
        public TRosMessage RosMessage { get; }

        public RosMessageReceivedEventArgs(TRosMessage message)
        {
            if (null == message)
            {
                throw new ArgumentNullException(nameof(message));
            }

            RosMessage = message;
        }
    }
}
