namespace Rosbridge.Client.Common.Interfaces
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// WebSocket connection handler
    /// </summary>
    public interface ISocket : IDisposable
    {
        /// <summary>
        /// Server URI
        /// </summary>
        Uri URI { get; }

        /// <summary>
        /// Is connected to server
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Open a server connection
        /// </summary>
        /// <returns></returns>
        Task ConnectAsync();

        /// <summary>
        /// Closes server connection
        /// </summary>
        /// <returns></returns>
        Task DisconnectAsync();

        /// <summary>
        /// Sends message to server
        /// </summary>
        /// <param name="buffer"></param>
        /// <returns></returns>
        Task SendAsync(byte[] buffer);

        /// <summary>
        /// Receive message from server
        /// </summary>
        /// <returns></returns>
        Task<byte[]> ReceiveAsync();
    }
}
