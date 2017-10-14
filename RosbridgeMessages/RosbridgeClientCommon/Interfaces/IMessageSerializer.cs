namespace RosbridgeMessages.RosbridgeClientCommon.Interfaces
{
    public interface IMessageSerializer
    {
        /// <summary>
        /// Serialize an object to byte array
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        byte[] Serialize(object message);

        /// <summary>
        /// Deserialize a byte array to the given type
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        TType Deserialize<TType>(byte[] buffer);
    }
}
