namespace RosbridgeClientCommon.Exceptions
{
    using System;

    public class RosMessageTypeAttributeNullException : Exception
    {
        public RosMessageTypeAttributeNullException() : base() { }
        public RosMessageTypeAttributeNullException(string message) : base(message) { }
        public RosMessageTypeAttributeNullException(string message, Exception innerException) : base(message, innerException) { }
    }
}
