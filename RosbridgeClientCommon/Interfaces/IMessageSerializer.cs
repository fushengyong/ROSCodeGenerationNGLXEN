namespace RosbridgeClientCommon.Interfaces
{
    public interface IMessageSerializer<TMessage>
    {
        /// <summary>
        /// Serialize an object to byte array
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        byte[] Serialize(TMessage message);

        /// <summary>
        /// Deserialize a byte array to the given type
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="buffer"></param>
        /// <returns></returns>
        TMessage Deserialize(byte[] buffer);
    }
}
