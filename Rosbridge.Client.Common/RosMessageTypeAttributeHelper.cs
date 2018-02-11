namespace Rosbridge.Client.Common
{
    using Rosbridge.Client.Common.Attributes;
    using Rosbridge.Client.Common.Exceptions;
    using Rosbridge.Client.Common.Interfaces;
    using System;
    using System.Linq;

    public class RosMessageTypeAttributeHelper : IRosMessageTypeAttributeHelper
    {
        public string GetRosMessageTypeFromTypeAttribute(Type rosMessageType)
        {
            if (null == rosMessageType)
            {
                throw new ArgumentNullException(nameof(rosMessageType));
            }

            object[] attributeCollection = rosMessageType.GetCustomAttributes(typeof(RosMessageTypeAttribute), false);

            RosMessageTypeAttribute rosMessageTypeAttribute = attributeCollection.FirstOrDefault() as RosMessageTypeAttribute;

            if (rosMessageTypeAttribute == null)
            {
                throw new RosMessageTypeAttributeNullException();
            }

            if (string.IsNullOrWhiteSpace(rosMessageTypeAttribute.RosMessageType))
            {
                throw new RosMessageTypeAttributeEmptyException();
            }

            return rosMessageTypeAttribute.RosMessageType;
        }
    }
}
