namespace RosbridgeMessages.RosbridgeClientCommon.Exceptions
{
    using System;

    public class MessageDispatcherException : Exception
    {
        public MessageDispatcherException() : base() { }
        public MessageDispatcherException(string message) : base(message) { }
        public MessageDispatcherException(string message, Exception innerException) : base(message, innerException) { }
    }
}
