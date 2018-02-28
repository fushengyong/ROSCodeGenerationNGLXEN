namespace Rosbridge.Client.V2_0.UnitTests
{
    using Exceptions;
    using FluentAssertions;
    using Interfaces;
    using Moq;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using System;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    [TestFixture]
    public class SocketUnitTests
    {
        private Mock<IClientWebSocket> _clientWebSocketMock;
        private CancellationTokenSource _cancellationTokenSource;

        [SetUp]
        public void SetUp()
        {
            _clientWebSocketMock = new Mock<IClientWebSocket>();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        [Test]
        public void Constructor_UnitTest_ArgumentsOK_FieldsSetCorrectly()
        {
            //arrange
            Uri uri = new Uri("http://localhost");

            //act
            Socket testClass = new Socket(uri, _clientWebSocketMock.Object, _cancellationTokenSource);

            //assert
            testClass._disposed.Should().BeFalse();
            testClass.URI.Should().BeSameAs(uri);
        }

        [Test]
        public void Constructor_UnitTest_UriIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            Uri uri = null;

            //act
            Action act = () => new Socket(uri, _clientWebSocketMock.Object, _cancellationTokenSource);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void Constructor_UnitTest_ClientWebSocketIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            Uri uri = new Uri("http://localhost");

            //act
            Action act = () => new Socket(uri, null, _cancellationTokenSource);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void Constructor_UnitTest_CancellationTokenSourceIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            Uri uri = new Uri("http://localhost");

            //act
            Action act = () => new Socket(uri, _clientWebSocketMock.Object, null);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public async Task ConnectAsync_UnitTest_DependenciesOK_ResultNotNullWebSocketConnectAsyncCalled()
        {
            //arrange
            Uri uri = new Uri("http://localhost");

            Socket testClass = new Socket(uri, _clientWebSocketMock.Object, _cancellationTokenSource);

            _clientWebSocketMock.SetupGet(webSocket => webSocket.State).Returns(WebSocketState.Open);

            //act
            Task result = testClass.ConnectAsync();

            //assert
            await result;
            result.Should().NotBeNull();
            result.Exception.Should().BeNull();
            _clientWebSocketMock.Verify(webSocket => webSocket.ConnectAsync(uri, _cancellationTokenSource.Token), Times.Once);
        }

        [Test]
        public void ConnectAsync_UnitTest_ObjectDisposed_ShouldThrowObjectDisposedException()
        {
            //arrange
            Uri uri = new Uri("http://localhost");

            Socket testClass = new Socket(uri, _clientWebSocketMock.Object, _cancellationTokenSource);
            testClass._disposed = true;

            _clientWebSocketMock.SetupGet(webSocket => webSocket.State).Returns(WebSocketState.Open);

            //act
            Action act = () => testClass.ConnectAsync();

            //assert
            act.Should().ThrowExactly<ObjectDisposedException>();
        }

        [Test]
        public void ConnectAsync_UnitTest_WebSocketNotInOpenState_ShouldThrowSocketException()
        {
            //arrange
            Uri uri = new Uri("http://localhost");

            Socket testClass = new Socket(uri, _clientWebSocketMock.Object, _cancellationTokenSource);

            _clientWebSocketMock.SetupGet(webSocket => webSocket.State).Returns(WebSocketState.Closed);

            //act
            Func<Task> act = async () => await testClass.ConnectAsync();

            //assert
            act.Should().ThrowExactly<SocketException>();
        }

        [Test]
        public async Task DisconnectAsync_UnitTest_DependenciesOK_ResultNotNullWebSocketConnectAsyncCalled()
        {
            //arrange
            Uri uri = new Uri("http://localhost");

            Socket testClass = new Socket(uri, _clientWebSocketMock.Object, _cancellationTokenSource);

            _clientWebSocketMock.SetupGet(webSocket => webSocket.State).Returns(WebSocketState.Closed);

            //act
            Task result = testClass.DisconnectAsync();

            //assert
            await result;
            result.Should().NotBeNull();
            result.Exception.Should().BeNull();
            _clientWebSocketMock.Verify(webSocket =>
                webSocket.CloseAsync(It.Is<WebSocketCloseStatus>(status => status == WebSocketCloseStatus.NormalClosure), It.IsAny<string>(), _cancellationTokenSource.Token), Times.Once);
        }

        [Test]
        public void DisconnectAsync_UnitTest_ObjectDisposed_ShouldThrowObjectDisposedException()
        {
            //arrange
            Uri uri = new Uri("http://localhost");

            Socket testClass = new Socket(uri, _clientWebSocketMock.Object, _cancellationTokenSource);
            testClass._disposed = true;

            _clientWebSocketMock.SetupGet(webSocket => webSocket.State).Returns(WebSocketState.Closed);

            //act
            Action act = () => testClass.DisconnectAsync();

            //assert
            act.Should().ThrowExactly<ObjectDisposedException>();
        }

        [Test]
        public void DisconnectAsync_UnitTest_WebSocketNotInClosedState_ShouldThrowSocketException()
        {
            //arrange
            Uri uri = new Uri("http://localhost");

            Socket testClass = new Socket(uri, _clientWebSocketMock.Object, _cancellationTokenSource);

            _clientWebSocketMock.SetupGet(webSocket => webSocket.State).Returns(WebSocketState.Open);

            //act
            Func<Task> act = async () => await testClass.DisconnectAsync();

            //assert
            act.Should().ThrowExactly<SocketException>();
        }

        [Test]
        public void SendAsync_UnitTest_ArgumentOK_ShouldCalledWebSocketSendAsync()
        {
            //arrange
            Uri uri = new Uri("http://localhost");
            byte[] buffer = new byte[5];

            Socket testClass = new Socket(uri, _clientWebSocketMock.Object, _cancellationTokenSource);

            //act
            Task resultTask = testClass.SendAsync(buffer);

            //assert
            resultTask.Should().NotBeNull();
            _clientWebSocketMock.Verify(webSocket => webSocket.SendAsync(It.IsAny<ArraySegment<byte>>(), It.Is<WebSocketMessageType>(messageType => messageType == WebSocketMessageType.Text), true, _cancellationTokenSource.Token), Times.Once);
        }

        [Test]
        public void SendAsync_UnitTest_ObjectDisposed_ShouldThrowObjectDisposedException()
        {
            //arrange
            Uri uri = new Uri("http://localhost");
            byte[] buffer = new byte[5];

            Socket testClass = new Socket(uri, _clientWebSocketMock.Object, _cancellationTokenSource);

            testClass._disposed = true;

            //act
            Action act = () => testClass.SendAsync(buffer);

            //assert
            act.Should().ThrowExactly<ObjectDisposedException>();
        }

        [Test]
        public void SendAsync_UnitTest_BufferIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            Uri uri = new Uri("http://localhost");
            byte[] buffer = null;

            Socket testClass = new Socket(uri, _clientWebSocketMock.Object, _cancellationTokenSource);

            //act
            Action act = () => testClass.SendAsync(buffer);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void Dispose_UnitTest_ObjectNotDisposed_ShouldCallWebSocketAbortAndDisposeMethod()
        {
            //arrange
            Uri uri = new Uri("http://localhost");

            Socket testClass = new Socket(uri, _clientWebSocketMock.Object, _cancellationTokenSource);

            //act
            testClass.Dispose();

            //assert
            testClass._disposed.Should().BeTrue();
            _clientWebSocketMock.Verify(webSocket => webSocket.Abort(), Times.Once);
            _clientWebSocketMock.Verify(webSocket => webSocket.Dispose(), Times.Once);
        }
    }
}
