namespace RosbridgeClientCommon.UnitTests
{
    using FluentAssertions;
    using Interfaces;
    using Moq;
    using NUnit.Framework;
    using RosbridgeClientCommon.Exceptions;
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    [TestFixture]
    public class MessageDispatcherUnitTests
    {
        private const string DISPOSED_FIELD_NAME = "_disposed";
        private const string CURRENT_STATE_PROPERTY_NAME = "CurrentState";

        private readonly Type _testClassType = typeof(MessageDispatcher);

        private Mock<MessageDispatcher> _testClassPartialMock;
        private Mock<ISocket> _socketMock;
        private Mock<IMessageSerializer> _serializerMock;

        [SetUp]
        public void SetUp()
        {
            _socketMock = new Mock<ISocket>();
            _serializerMock = new Mock<IMessageSerializer>();
            _testClassPartialMock = new Mock<MessageDispatcher>(_socketMock.Object, _serializerMock.Object);
        }

        private FieldInfo GetPrivateFieldOfTestObject(string fieldName)
        {
            return _testClassType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private PropertyInfo GetPropertyOfTestObject(string propertyName)
        {
            return _testClassType.GetProperty(propertyName);
        }

        [Test]
        public void StartAsync_DispatcherDisposed_ShouldThrowObjectDisposedException()
        {
            //arrange
            GetPrivateFieldOfTestObject(DISPOSED_FIELD_NAME).SetValue(_testClassPartialMock.Object, true);

            //act
            Func<Task> act = async () => await _testClassPartialMock.Object.StartAsync();

            //assert
            act.ShouldThrow<ObjectDisposedException>();
            _socketMock.Verify(socket => socket.ConnectAsync(), Times.Never);
            _socketMock.Verify(socket => socket.ReceiveAsync(), Times.Never);
        }

        [Test]
        public void StartAsync_DispatcherNotInStoppedState_ShouldThrowMessageDispatcherException()
        {
            //arrange
            GetPropertyOfTestObject(CURRENT_STATE_PROPERTY_NAME).SetValue(_testClassPartialMock.Object, Enums.States.Started);

            //act
            Func<Task> act = async () => await _testClassPartialMock.Object.StartAsync();

            //assert
            act.ShouldThrow<MessageDispatcherException>();
            _socketMock.Verify(socket => socket.ConnectAsync(), Times.Never);
            _socketMock.Verify(socket => socket.ReceiveAsync(), Times.Never);
        }

        [Test]
        public void StartAsync_SocketThrowsException_MessageDispatcherShouldBeStopped()
        {
            //arrange
            _socketMock.Setup(socket => socket.ConnectAsync()).Throws<Exception>();

            //act
            Func<Task> act = async () => { await _testClassPartialMock.Object.StartAsync(); };

            //assert
            act.ShouldThrow<AggregateException>();
            _testClassPartialMock.Object.CurrentState.Should().Be(Enums.States.Stopped);
            _socketMock.Verify(socket => socket.ConnectAsync(), Times.Once());
            _socketMock.Verify(socket => socket.ReceiveAsync(), Times.Never());
        }

        [Test]
        public async Task StartAsync_SocketDidNotThrowException_MessageDispatcherShouldBeStarted()
        {
            //arrange

            //act
            await _testClassPartialMock.Object.StartAsync();

            //assert
            _testClassPartialMock.Object.CurrentState.Should().Be(Enums.States.Started);
            _socketMock.Verify(socket => socket.ConnectAsync(), Times.Once());
        }

        [Test]
        public void StopAsync_DispatcherDisposed_ShouldThrowObjectDisposedException()
        {
            //arrange
            GetPrivateFieldOfTestObject(DISPOSED_FIELD_NAME).SetValue(_testClassPartialMock.Object, true);

            //act
            Func<Task> act = async () => await _testClassPartialMock.Object.StopAsync();

            //assert
            act.ShouldThrow<ObjectDisposedException>();
        }

        [Test]
        public void StopAsync_DispatcherNotInStartedState_ShouldThrowMessageDispatcherException()
        {
            //arrange

            //act
            Func<Task> act = async () => await _testClassPartialMock.Object.StopAsync();

            //assert
            act.ShouldThrow<MessageDispatcherException>();
        }

        [Test]
        public async Task StopAsync_DispatcherInStartedState_ShouldBeInStoppedState()
        {
            //arrange
            GetPropertyOfTestObject(CURRENT_STATE_PROPERTY_NAME).SetValue(_testClassPartialMock.Object, Enums.States.Started);

            //act
            await _testClassPartialMock.Object.StopAsync();

            //assert
            _testClassPartialMock.Object.CurrentState.Should().Be(Enums.States.Stopped);
            _socketMock.Verify(socket => socket.DisconnectAsync(), Times.Once);
        }

        [Test]
        public void SendAsync_DispatcherIsDisposed_ShouldThrowObjectDisposedException()
        {
            //arrange
            Object message = new object();

            GetPrivateFieldOfTestObject(DISPOSED_FIELD_NAME).SetValue(_testClassPartialMock.Object, true);

            //act
            Func<Task> act = async () => await _testClassPartialMock.Object.SendAsync(message);

            //assert
            act.ShouldThrow<ObjectDisposedException>();
        }

        [Test]
        public void SendAsync_DispatcherNotInStartedState_ShouldThrowMessageDispatcherException()
        {
            //arrange
            Object message = new object();

            //act
            Func<Task> act = async () => await _testClassPartialMock.Object.SendAsync(message);

            //assert
            act.ShouldThrow<MessageDispatcherException>();
        }

        [Test]
        public async Task SendAsync_DispatcherInStartedState_SocketSendAsyncSerializerSerializeShouldCalled()
        {
            //arrange
            Object message = new object();
            byte[] messageByteArray = new byte[5];

            _serializerMock.Setup(serializer => serializer.Serialize(message)).Returns(messageByteArray);
            GetPropertyOfTestObject(CURRENT_STATE_PROPERTY_NAME).SetValue(_testClassPartialMock.Object, Enums.States.Started);

            //act
            await _testClassPartialMock.Object.SendAsync(message);

            //assert
            _serializerMock.Verify(serializer => serializer.Serialize(message), Times.Once);
            _socketMock.Verify(socket => socket.SendAsync(messageByteArray), Times.Once);
        }

        [Test]
        public void Dispose_DispatcherAlreadyDisposed_CallNothing()
        {
            //arrange
            Enums.States dispatcherState = Enums.States.Started;
            GetPrivateFieldOfTestObject(DISPOSED_FIELD_NAME).SetValue(_testClassPartialMock.Object, true);
            GetPropertyOfTestObject(CURRENT_STATE_PROPERTY_NAME).SetValue(_testClassPartialMock.Object, dispatcherState);

            //act
            _testClassPartialMock.Object.Dispose();

            //assert
            _testClassPartialMock.Object.CurrentState.Should().Be(dispatcherState);
        }

        [Test]
        public void Dispose_DispatcherNotDisposed()
        {
            //arrange

            //act
            _testClassPartialMock.Object.Dispose();

            //assert
            _socketMock.Verify(socket => socket.DisconnectAsync(), Times.Once);
            _socketMock.Verify(socket => socket.Dispose(), Times.Once);
        }

        [Test]
        public void GetUniqueId()
        {
            //arrange
            Guid uniqueId = Guid.NewGuid();

            //act
            string uniqueIdReuslt = _testClassPartialMock.Object.GetUniqueID();

            //assert
            uniqueIdReuslt.Should().NotBeNull();
            uniqueIdReuslt.Should().NotBe(uniqueId.ToString());
        }
    }
}
