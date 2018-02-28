namespace Rosbridge.Client.V2_0
{
    using Exceptions;
    using Interfaces;
    using Rosbridge.Client.Common.Interfaces;
    using System;
    using System.IO;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    public class Socket : ISocket
    {
        private readonly IClientWebSocket _webSocket;
        private readonly CancellationTokenSource _cancellationTokenSource;
        protected internal bool _disposed;

        public bool IsConnected
        {
            get
            {
                return _webSocket.State == WebSocketState.Open;
            }
        }

        public Uri URI { get; private set; }

        public Socket(Uri uri, IClientWebSocket webSocket, CancellationTokenSource cancellationTokenSource)
        {
            if (null == uri)
            {
                throw new ArgumentNullException(nameof(uri));
            }

            if (null == webSocket)
            {
                throw new ArgumentNullException(nameof(webSocket));
            }

            if (null == cancellationTokenSource)
            {
                throw new ArgumentNullException(nameof(cancellationTokenSource));
            }

            _webSocket = webSocket;
            _cancellationTokenSource = cancellationTokenSource;
            _disposed = false;
            URI = uri;
        }

        public Task ConnectAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(Socket));
            }

            return Task.Run(async () =>
            {
                await _webSocket.ConnectAsync(URI, _cancellationTokenSource.Token);

                if (_webSocket.State != WebSocketState.Open)
                {
                    throw new SocketException("Could not connect to " + URI + " (" + _webSocket.State.ToString() + ")");
                }
            });
        }

        public Task DisconnectAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(Socket));
            }

            return Task.Run(async () =>
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Close", _cancellationTokenSource.Token);

                if (_webSocket.State != WebSocketState.Closed)
                {
                    throw new SocketException("Could not connect to " + URI + " (" + _webSocket.State.ToString() + ")");
                }
            });
        }

        public Task<byte[]> ReceiveAsync()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(Socket));
            }

            TaskCompletionSource<byte[]> taskCompletion = new TaskCompletionSource<byte[]>();

            Task.Run(async () =>
            {
                using (MemoryStream buffer = new MemoryStream())
                {
                    WebSocketReceiveResult result;

                    do
                    {
                        byte[] tmpBuffer = new byte[65535];

                        result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(tmpBuffer), _cancellationTokenSource.Token);

                        if (null != result)
                        {
                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK", _cancellationTokenSource.Token);
                                taskCompletion.SetException(new SocketException("Connection closed by the server!"));
                            }

                            if (result.MessageType == WebSocketMessageType.Binary)
                            {
                                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK", _cancellationTokenSource.Token);
                                taskCompletion.SetException(new SocketException("Server sent binary data!"));
                            }

                            buffer.Write(tmpBuffer, 0, result.Count);
                        }
                    } while (null != result && !result.EndOfMessage);

                    taskCompletion.SetResult(buffer.ToArray());
                }
            });

            return taskCompletion.Task;
        }

        public Task SendAsync(byte[] buffer)
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(Socket));
            }

            if (null == buffer)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            return _webSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, _cancellationTokenSource.Token);
        }

        public void Dispose()
        {
            _disposed = true;

            try
            {
                if (null != _webSocket)
                {
                    _webSocket.Abort();
                    _webSocket.Dispose();
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
