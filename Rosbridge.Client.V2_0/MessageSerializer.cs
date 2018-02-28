namespace Rosbridge.Client.V2_0
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Rosbridge.Client.Common.Interfaces;
    using System;
    using System.Text;

    public class MessageSerializer : IMessageSerializer
    {
        public JObject Deserialize(byte[] buffer)
        {
            if (null == buffer)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            string jsonString = Encoding.ASCII.GetString(buffer, 0, buffer.Length);

            return JObject.Parse(jsonString);
        }

        public byte[] Serialize<TMessage>(TMessage message) where TMessage : class, new()
        {
            if (null == message)
            {
                throw new ArgumentNullException(nameof(message));
            }

            string jsonString = JsonConvert.SerializeObject(message);

            return Encoding.ASCII.GetBytes(jsonString);
        }
    }
}
