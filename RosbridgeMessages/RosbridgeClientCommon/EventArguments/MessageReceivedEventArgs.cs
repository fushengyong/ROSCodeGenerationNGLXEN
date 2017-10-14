namespace RosbridgeMessages.RosbridgeClientCommon.EventArguments
{
    using System;

    public class MessageReceivedEventArgs<TMessage> : EventArgs
    {
        public TMessage Message { get; }

        public MessageReceivedEventArgs(TMessage message)
        {
            Message = message;
        }
    }
}
