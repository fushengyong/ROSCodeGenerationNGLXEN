namespace Rosbridge.Client.V2_0.IntegrationTests
{
    using Common;
    using Common.Interfaces;
    using Interfaces;
    using Messages.RosOperations;
    using Moq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using System;
    using System.Linq;
    using System.Net.WebSockets;
    using System.Threading;
    using System.Threading.Tasks;

    [TestFixture]
    public class PublisherIntegrationTests
    {
        private const string URI = "http://localhost";
        private const string TOPIC = "testTopic";
        private const string TYPE = "testType";

        private Mock<IClientWebSocket> _clientWebSocketMock;
        private Mock<IRosMessageTypeAttributeHelper> _rosMessageTypeAttributeHelperMock;
        private Uri _uri;
        private CancellationTokenSource _cancellationTokenSource;
        private ISocket _socket;
        private IMessageSerializer _messageSerializer;
        private IMessageDispatcher _messageDispatcher;
        private RosPublisher<object> _publisher;

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

            _publisher = new RosPublisher<object>(TOPIC, _messageDispatcher, _rosMessageTypeAttributeHelperMock.Object);
        }

        [Test]
        public async Task AdvertiseAsync_MessageDispatcherStarted_WebSocketShouldSendCorrectByteArray()
        {
            //arrange
            RosAdvertiseMessage rosAdvertiseMessage = new RosAdvertiseMessage()
            {
                Id = _publisher._uniqueId,
                Topic = TOPIC,
                Type = TYPE
            };

            byte[] serialized = _messageSerializer.Serialize(rosAdvertiseMessage);

            _clientWebSocketMock.SetupGet(clientWebSocket => clientWebSocket.State).Returns(WebSocketState.Open);

            await _messageDispatcher.StartAsync();

            //act
            await _publisher.AdvertiseAsync();

            //assert
            _clientWebSocketMock.Verify(clientWebSocket => clientWebSocket.SendAsync(
                It.Is<ArraySegment<byte>>(arraySegment => arraySegment.Array.SequenceEqual(serialized)),
                WebSocketMessageType.Text,
                true,
                _cancellationTokenSource.Token));
        }

        [Test]
        public async Task UnadvertiseAsync_MessageDispatcherStarted_WebSocketShouldSendCorrectByteArray()
        {
            //arrange
            RosUnadvertiseMessage rosUnadvertiseMessage = new RosUnadvertiseMessage()
            {
                Id = _publisher._uniqueId,
                Topic = TOPIC
            };

            byte[] serialized = _messageSerializer.Serialize(rosUnadvertiseMessage);

            _clientWebSocketMock.SetupGet(clientWebSocket => clientWebSocket.State).Returns(WebSocketState.Open);

            await _messageDispatcher.StartAsync();

            //act
            await _publisher.UnadvertiseAsync();

            //assert
            _clientWebSocketMock.Verify(clientWebSocket => clientWebSocket.SendAsync(
                It.Is<ArraySegment<byte>>(arraySegment => arraySegment.Array.SequenceEqual(serialized)),
                WebSocketMessageType.Text,
                true,
                _cancellationTokenSource.Token));
        }

        [Test]
        public async Task SendAsync_MessageDispatcherStarted_WebSocketShouldSendCorrectByteArray()
        {
            //arrange
            object message = new { Test = "testMessage" };

            JObject jsonMessage = JObject.FromObject(message);

            RosPublishMessage rosPublishMessage = new RosPublishMessage()
            {
                Id = _publisher._uniqueId,
                Topic = TOPIC,
                Message = jsonMessage
            };

            byte[] serialized = _messageSerializer.Serialize(rosPublishMessage);

            _clientWebSocketMock.SetupGet(clientWebSocket => clientWebSocket.State).Returns(WebSocketState.Open);

            await _messageDispatcher.StartAsync();

            //act
            await _publisher.PublishAsync(message);

            //assert
            _clientWebSocketMock.Verify(clientWebSocket => clientWebSocket.SendAsync(
                It.Is<ArraySegment<byte>>(arraySegment => arraySegment.Array.SequenceEqual(serialized)),
                WebSocketMessageType.Text,
                true,
                _cancellationTokenSource.Token));
        }
    }
}