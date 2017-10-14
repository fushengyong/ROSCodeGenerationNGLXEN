namespace RosbridgeMessages.RosbridgeClientCommon.Interfaces
{
    using System;
    using System.Threading.Tasks;

    public interface ISocket : IDisposable
    {
        /// <summary>
        /// Rosbridge server URI.
        /// </summary>
        Uri Uri { get; }

        /// <summary>
        /// True if connected to Rosbridge
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// Open a Rosbridge server connection
        /// </summary>
        /// <returns></returns>
        Task ConnectedAsync();

        /// <summary>
        /// Coloses Rosbridge server connection
        /// </summary>
        /// <returns></returns>
        Task DisconnectedAsync();

        /// <summary>
        /// Sends message to Rosbridge server
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        Task SendAsync(byte[] buffer);

        /// <summary>
        /// Receive message from Rosbridge server
        /// </summary>
        /// <returns></returns>
        Task<byte[]> ReceiveAsync();
    }
}
