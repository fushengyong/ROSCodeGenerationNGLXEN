namespace Rosbridge.Client.Common.Interfaces
{
    using System;

    /// <summary>
    /// Helps extract ROS message type from any type
    /// </summary>
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
