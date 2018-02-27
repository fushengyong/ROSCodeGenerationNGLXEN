namespace Rosbridge.Client.Common
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
        protected internal bool _disposed;
        protected internal Task _receivingTask;
        protected internal Task _disposingTask;
        protected internal States _currentState;

        public event MessageReceivedHandler MessageReceived;

        public States CurrentState
        {
            get { return _currentState; }
        }

        public string GetNewUniqueID()
        {
            return Guid.NewGuid().ToString();
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
            _currentState = States.Stopped;
        }

        public Task StartAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MessageDispatcher));
            }

            if (_currentState != States.Stopped)
            {
                throw new MessageDispatcherException("Dispatcher is not stopped!");
            }

            Task socketConnectTask = Task.Run(async () =>
            {
                _currentState = States.Starting;

                try
                {
                    await _socket.ConnectAsync();
                }
                catch
                {
                    _currentState = States.Stopped;
                    throw;
                }

                _currentState = States.Started;
            });

            _receivingTask = socketConnectTask.ContinueWith(async (socketTask) =>
            {
                while (_socket.IsConnected && _currentState == States.Started)
                {
                    byte[] buffer = await _socket.ReceiveAsync();

                    JObject message = _serializer.Deserialize(buffer);

                    MessageReceived?.Invoke(this, new RosbridgeMessageReceivedEventArgs(message));
                }
            });

            return socketConnectTask;
        }

        public Task StopAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MessageDispatcher));
            }

            if (_currentState != States.Started)
            {
                throw new MessageDispatcherException("Dispatcher is not started!");
            }

            _currentState = States.Stopping;

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

                _currentState = States.Stopped;
            });
        }

        public Task SendAsync<TMessage>(TMessage message) where TMessage : class, new()
        {
            if (null == message)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(MessageDispatcher));
            }

            if (CurrentState != States.Started)
            {
                throw new MessageDispatcherException("Dispatcher is not started!");
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
            _currentState = States.Stopped;

            _disposingTask = Task.Run(async () =>
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

            GC.SuppressFinalize(this);
        }
    }
}
