namespace Rosbridge.Client.Common.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
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
