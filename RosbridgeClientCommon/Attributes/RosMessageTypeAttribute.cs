namespace RosbridgeClientCommon.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class)]
    public class RosMessageTypeAttribute : Attribute
    {
        public string RosMessageType { get; private set; }

        public RosMessageTypeAttribute(string type)
        {
            if (null == type)
            {
                throw new ArgumentNullException(nameof(type));
            }

            RosMessageType = type;
        }
    }
}
