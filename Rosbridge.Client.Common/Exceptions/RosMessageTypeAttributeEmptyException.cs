namespace Rosbridge.Client.Common.Exceptions
{
    using System;

    public class RosMessageTypeAttributeEmptyException : Exception
    {
        private const string MESSAGE_TEMPLATE = "The attribute value cannot be empty on class: {0}!";

        public RosMessageTypeAttributeEmptyException() : base() { }
        public RosMessageTypeAttributeEmptyException(string classType) : base(string.Format(MESSAGE_TEMPLATE, classType)) { }
        public RosMessageTypeAttributeEmptyException(string classType, Exception innerException) : base(string.Format(MESSAGE_TEMPLATE, classType), innerException) { }
    }
}
