namespace RosbridgeClientCommon.Interfaces
{
    using Enums;
    using EventArguments;
    using System;
    using System.Threading.Tasks;

    public delegate void MessageReceivedHandler(object sender, RosbridgeMessageReceivedEventArgs args);

    public interface IMessageDispatcher : IDisposable
    {
        /// <summary>
        /// Event for handling received messages
        /// </summary>
        event MessageReceivedHandler MessageReceived;

        /// <summary>
        /// Dispatcher status
        /// </summary>
        States CurrentState { get; }

        /// <summary>
        /// Start dispatcher
        /// </summary>
        /// <returns></returns>
        Task StartAsync();

        /// <summary>
        /// Stop dispatcher 
        /// </summary>
        /// <returns></returns>
        Task StopAsync();

        /// <summary>
        /// Send a message asynchronously
        /// </summary>
        /// <returns></returns>
        Task SendAsync<TMessage>(TMessage message) where TMessage : class, new();

        /// <summary>
        /// Returns Unique ID
        /// </summary>
        /// <returns></returns>
        string GetNewUniqueID();
    }
}
