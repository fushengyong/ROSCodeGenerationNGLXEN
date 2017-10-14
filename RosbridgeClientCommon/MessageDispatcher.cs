namespace RosbridgeClientCommon
{
    using Enums;
    using EventArguments;
    using Exceptions;
    using Interfaces;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Threading.Tasks;

    public class MessageDispatcher : IMessageDispatcher
    {
        private ISocket _socket;
        private IMessageSerializer _serializer;
        private bool _disposed;
        private Task _receivingTask;

        public event MessageReceivedHandler MessageReceived;

        public States CurrentState
        {
            get; private set;
        }

        public MessageDispatcher(ISocket socket, IMessageSerializer serializer)
        {
            if (null == socket)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            if (null == serializer)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            _socket = socket;
            _serializer = serializer;
            _disposed = false;
            CurrentState = States.Stopped;
        }

        public Task StartAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(_socket));
            }

            if (CurrentState != States.Stopped)
            {
                throw new MessageDispatcherException("Message dispatcher is not stopped!");
            }

            CurrentState = States.Starting;


            //TODO SIMPLIFY
            Task socketConnectTask = Task.Run(async () =>
            {
                try
                {
                    await _socket.ConnectAsync();
                }
                catch
                {
                    CurrentState = States.Stopped;
                    throw;
                }

                CurrentState = States.Started;
            });

            _receivingTask = Task.Run(async () =>
            {
                try
                {
                    await socketConnectTask;
                }
                catch
                {
                    return;
                }

                byte[] buffer;
                while (CurrentState == States.Started)
                {
                    try
                    {
                        buffer = await _socket.ReceiveAsync();

                        JObject message = _serializer.Deserialize(buffer);

                        MessageReceived?.Invoke(this, new RosbridgeMessageReceivedEventArgs(message));
                    }
                    catch
                    {

                    }
                }
            });

            return socketConnectTask;
        }

        public Task StopAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(_socket));
            }

            if (CurrentState != States.Started)
            {
                throw new MessageDispatcherException("Dispatcher not in 'started' state!");
            }

            CurrentState = States.Stopping;

            return Task.Run(async () =>
            {
                if (null != _socket)
                {
                    await _socket.DisconnectAsync();
                }

                if (null != _receivingTask)
                {
                    await _receivingTask;
                    _receivingTask = null;
                }

                CurrentState = States.Stopped;
            });
        }

        public Task SendAsync<TMessage>(TMessage message) where TMessage : class, new()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(_socket));
            }

            if (CurrentState != States.Started)
            {
                throw new MessageDispatcherException("Dispatcher not in 'started' state!");
            }

            return _socket.SendAsync(_serializer.Serialize(message));
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            CurrentState = States.Stopped;

            Task.Run(async () =>
            {
                try
                {
                    if (null != _socket)
                    {
                        await _socket.DisconnectAsync();
                    }

                    if (null != _receivingTask)
                    {
                        await _receivingTask;
                        _receivingTask = null;
                    }

                    if (null != _socket)
                    {
                        _socket.Dispose();
                        _socket = null;
                    }

                    if (null != _serializer)
                    {
                        _serializer = null;
                    }
                }
                catch
                {

                }
            });
        }

        public string GetUID()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
