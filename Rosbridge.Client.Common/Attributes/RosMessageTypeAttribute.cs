namespace Rosbridge.Client.Common.Attributes
{
    using System;

    /// <summary>
    /// Use this attribute to specify the ROS message type that the class represents
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RosMessageTypeAttribute : Attribute
    {
        public string RosMessageType { get; private set; }

        public RosMessageTypeAttribute(string rosMessageType)
        {
            if (null == rosMessageType)
            {
                throw new ArgumentNullException(nameof(rosMessageType));
            }

            RosMessageType = rosMessageType;
        }
    }
}
