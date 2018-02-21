namespace Rosbridge.Client.Common.EventArguments
{
    using System;

    /// <summary>
    /// Contains the received ROS message object
    /// </summary>
    /// <typeparam name="TRosMessage"></typeparam>
    public class RosMessageReceivedEventArgs<TRosMessage> : EventArgs where TRosMessage : class, new()
    {
        public TRosMessage RosMessage { get; private set; }

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
