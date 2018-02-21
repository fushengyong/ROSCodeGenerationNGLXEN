namespace Rosbridge.Client.Common.Interfaces
{
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// JSON serializer/deserializer
    /// </summary>
    public interface IMessageSerializer
    {
        /// <summary>
        /// Serialize an object to byte array
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        byte[] Serialize<TMessage>(TMessage message) where TMessage : class, new();

        /// <summary>
        /// Deserialize a byte array to the given type
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        JObject Deserialize(byte[] buffer);
    }
}
