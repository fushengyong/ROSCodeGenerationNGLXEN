namespace RosbridgeClientCommon.Interfaces
{
    using Enums;
    using EventArguments;
    using System;
    using System.Threading.Tasks;

    public delegate void MessageReceivedHandler<TMessage>(object sender, MessageReceivedEventArgs<TMessage> message);

    public interface IMessageDispatcher<TMessage> : IDisposable
    {
        /// <summary>
        /// Event for handling received messages
        /// </summary>
        event MessageReceivedHandler<TMessage> MessageReceived;

        /// <summary>
        /// Dispatcher status
        /// </summary>
        States CurrentState { get; }

        /// <summary>
        /// Starts dispatcher
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
        Task SendAsync(TMessage message);
    }
}
