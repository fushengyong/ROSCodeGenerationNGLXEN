namespace RosbridgeClientCommon.Exceptions
{
    using System;

    public class RosMessageTypeAttributeEmptyException : Exception
    {
        public RosMessageTypeAttributeEmptyException() : base() { }
        public RosMessageTypeAttributeEmptyException(string message) : base(message) { }
        public RosMessageTypeAttributeEmptyException(string message, Exception innerException) : base(message, innerException) { }
    }
}
