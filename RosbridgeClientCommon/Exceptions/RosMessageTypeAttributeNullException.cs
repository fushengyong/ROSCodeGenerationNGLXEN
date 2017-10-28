namespace RosbridgeClientCommon.Exceptions
{
    using System;

    public class RosMessageTypeAttributeNullException : Exception
    {
        private const string MESSAGE_TEMPLATE = "The class {0} does not have the required attribute!";

        public RosMessageTypeAttributeNullException() : base() { }
        public RosMessageTypeAttributeNullException(string classType) : base(string.Format(MESSAGE_TEMPLATE, classType)) { }
        public RosMessageTypeAttributeNullException(string classType, Exception innerException) : base(string.Format(MESSAGE_TEMPLATE, classType), innerException) { }
    }
}
