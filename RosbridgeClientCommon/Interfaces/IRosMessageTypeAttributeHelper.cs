namespace RosbridgeClientCommon.Interfaces
{
    using System;

    public interface IRosMessageTypeAttributeHelper
    {
        /// <summary>
        /// Get ROS message type from given type attributes
        /// </summary>
        /// <param name="rosMessageType"></param>
        /// <returns>ROS message type as string</returns>
        string GetRosMessageTypeFromTypeAttribute(Type rosMessageType);
    }
}
