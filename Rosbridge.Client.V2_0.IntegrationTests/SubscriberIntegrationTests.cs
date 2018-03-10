namespace Rosbridge.Client.V2_0.IntegrationTests
{
    using Common;
    using Common.Interfaces;
    using Interfaces;
    using Messages.RosOperations;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    [TestFixture]
    public class SubscriberIntegrationTests
    {
        private const string URI = "http://localhost";
        private const string TOPIC = "testTopic";
        private const string TYPE = "testType";

        private Mock<IClientWebSocket> _clientWebSocketMock;
        private Mock<IRosMessageTypeAttributeHelper> _rosMessageTypeAttributeHelperMock;
        private Uri _uri;
        private CancellationTokenSource _cancellationTokenSource;
        private Socket _socket;
        private MessageSerializer _messageSerializer;
        private MessageDispatcher _messageDispatcher;
        private Subscriber<object> _subscriber;

        [SetUp]
        public void SetUp()
        {
            _clientWebSocketMock = new Mock<IClientWebSocket>();
            _rosMessageTypeAttributeHelperMock = new Mock<IRosMessageTypeAttributeHelper>();
            _uri = new Uri(URI);
            _cancellationTokenSource = new CancellationTokenSource();
            _socket = new Socket(_uri, _clientWebSocketMock.Object, _cancellationTokenSource);
            _messageSerializer = new MessageSerializer();
            _messageDispatcher = new MessageDispatcher(_socket, _messageSerializer);

            _rosMessageTypeAttributeHelperMock
                .Setup(helper => helper.GetRosMessageTypeFromTypeAttribute(typeof(object))).Returns(TYPE);

            _subscriber = new Subscriber<object>(TOPIC, _messageDispatcher, _rosMessageTypeAttributeHelperMock.Object);
        }

        [Test]
        public async Task SubscribeAsync_MessageDispatcherStarted_WebSocketShouldSendCorrectByteArray()
        {
            //arrange
            RosSubscribeMessage rosAdvertiseMessage = new RosSubscribeMessage()
            {
                Id = _subscriber._uniqueId,
                Topic = TOPIC,
                Type = TYPE
            };

            byte[] serialized = _messageSerializer.Serialize(rosAdvertiseMessage);

            _clientWebSocketMock.SetupGet(clientWebSocket => clientWebSocket.State).Returns(WebSocketState.Open);

            await _messageDispatcher.StartAsync();

            //act
            await _subscriber.SubscribeAsync();

            //assert
            _clientWebSocketMock.Verify(clientWebSocket => clientWebSocket.SendAsync(
                It.Is<ArraySegment<byte>>(arraySegment => arraySegment.Array.SequenceEqual(serialized)),
                WebSocketMessageType.Text,
                true,
                _cancellationTokenSource.Token));
        }

        [Test]
        public async Task UnsubscribeAsync_MessageDispatcherStarted_WebSocketShouldSendCorrectByteArray()
        {
            //arrange
            RosUnsubscribeMessage rosAdvertiseMessage = new RosUnsubscribeMessage()
            {
                Id = _subscriber._uniqueId,
                Topic = TOPIC
            };

            byte[] serialized = _messageSerializer.Serialize(rosAdvertiseMessage);

            _clientWebSocketMock.SetupGet(clientWebSocket => clientWebSocket.State).Returns(WebSocketState.Open);

            await _messageDispatcher.StartAsync();

            //act
            await _subscriber.UnsubscribeAsync();

            //assert
            _clientWebSocketMock.Verify(clientWebSocket => clientWebSocket.SendAsync(
                It.Is<ArraySegment<byte>>(arraySegment => arraySegment.Array.SequenceEqual(serialized)),
                WebSocketMessageType.Text,
                true,
                _cancellationTokenSource.Token));
        }
    }
}
