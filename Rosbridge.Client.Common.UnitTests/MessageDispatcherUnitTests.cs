namespace RosbridgeClientCommon.UnitTests
{
    using FluentAssertions;
    using Moq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using Rosbridge.Client.Common;
    using Rosbridge.Client.Common.Enums;
    using Rosbridge.Client.Common.Exceptions;
    using Rosbridge.Client.Common.Interfaces;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    public class MessageDispatcherUnitTests
    {
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

        [Test]
        public void GetNewUniqueID_UnitTest_ShouldBeGuid()
        {
            //arrange

            //act
            string result = _testClassPartialMock.Object.GetNewUniqueID();

            //assert
            result.Should().NotBeNull();
            Guid parsed;
            Guid.TryParse(result, out parsed).Should().BeTrue();
        }

        [Test]
        public void StartAsync_UnitTest_ClassDisposed_ShouldThrowObjectDisposedException()
        {
            //arrange
            _testClassPartialMock.Object._disposed = true;

            //act
            Action act = () => _testClassPartialMock.Object.StartAsync();

            //assert
            act.Should().ThrowExactly<ObjectDisposedException>();

            _socketMock.Verify(socket => socket.ConnectAsync(), Times.Never());
            _socketMock.Verify(socket => socket.ReceiveAsync(), Times.Never());
        }

        [Test]
        public void StartAsync_UnitTest_CurrentStateIsNotStopped_ShouldThrowMessageDispatcherException()
        {
            //arrange
            _testClassPartialMock.Object._currentState = States.Started;

            //act
            Action act = () => _testClassPartialMock.Object.StartAsync();

            //assert
            act.Should().ThrowExactly<MessageDispatcherException>();

            _socketMock.Verify(socket => socket.ConnectAsync(), Times.Never());
            _socketMock.Verify(socket => socket.ReceiveAsync(), Times.Never());
        }

        [Test]
        public async Task StartAsync_UnitTest_EverythingOk_DispatcherShouldStarted()
        {
            //arrange
            _testClassPartialMock.Object._currentState = States.Stopped;
            _socketMock.SetupGet(socket => socket.IsConnected).Returns(true);

            //act
            Task result = _testClassPartialMock.Object.StartAsync();

            //assert
            await result;
            result.Should().NotBeNull();
            result.IsCompleted.Should().BeTrue();
            _testClassPartialMock.Object.CurrentState.Should().Be(States.Started);

            _socketMock.Verify(socket => socket.ConnectAsync(), Times.Once());
        }

        [Test]
        public void StartAsync_UnitTest_SocketThrowException_ShouldThrowExceptionDispatcherShouldStopped()
        {
            //arrange
            _testClassPartialMock.Object._currentState = States.Stopped;
            _socketMock.Setup(socket => socket.ConnectAsync()).Throws<InvalidOperationException>();

            //act
            Func<Task> act = () => _testClassPartialMock.Object.StartAsync();

            //assert
            act.Should().ThrowExactly<InvalidOperationException>();
            _testClassPartialMock.Object.CurrentState.Should().Be(States.Stopped);

            _socketMock.Verify(socket => socket.ConnectAsync(), Times.Once());
        }

        [Test]
        public async Task StartAsync_UnitTest_EverythinkOk_ShouldReceiveEvent()
        {
            //arrange
            byte[] bufferMock = new byte[1];
            Task<byte[]> receiveAsyncTask = new Task<byte[]>(() => { return bufferMock; });
            JObject messageMock = new JObject();
            bool eventRaised = false;
            MessageReceivedHandler eventHandler = (sender, args) => { eventRaised = true; };
            _testClassPartialMock.Object.MessageReceived += eventHandler;

            _testClassPartialMock.Object._currentState = States.Stopped;
            _socketMock.SetupGet(socket => socket.IsConnected).Returns(true);
            _socketMock.Setup(socket => socket.ReceiveAsync()).Returns(receiveAsyncTask);
            _serializerMock.Setup(serializer => serializer.Deserialize(bufferMock)).Returns(messageMock);

            //act
            Task result = _testClassPartialMock.Object.StartAsync();

            //assert
            await result;
            _testClassPartialMock.Object.CurrentState.Should().Be(States.Started);

            _socketMock.Verify(socket => socket.ConnectAsync(), Times.Once());
        }

        [Test]
        public void StopAsync_UnitTest_ObjectDisposed_ShouldThrowObjectDisposedException()
        {
            //arrange
            _testClassPartialMock.Object._disposed = true;

            //act
            Action act = () => _testClassPartialMock.Object.StopAsync();

            //assert
            act.Should().ThrowExactly<ObjectDisposedException>();
        }

        [Test]
        public void StopAsync_UnitTest_ObjectNotInStartedState_ShouldThrowMessageDispatcherException()
        {
            //arrange
            _testClassPartialMock.Object._currentState = States.Stopped;

            //act
            Action act = () => _testClassPartialMock.Object.StopAsync();

            //assert
            act.Should().ThrowExactly<MessageDispatcherException>();
        }

        [Test]
        public void StopAsync_UnitTest_NotWait_ObjectShouldBeInStoppingState()
        {
            //arrange
            _testClassPartialMock.Object._currentState = States.Started;

            //act
            Task result = _testClassPartialMock.Object.StopAsync();

            //assert
            _testClassPartialMock.Object.CurrentState.Should().Be(States.Stopping);
        }

        [Test]
        public async Task StopAsync_UnitTest_Wait_ObjectShouldBeInStoppedState()
        {
            //arrange
            _testClassPartialMock.Object._currentState = States.Started;

            //act
            Task result = _testClassPartialMock.Object.StopAsync();

            //assert
            await result;
            result.IsCompleted.Should().BeTrue();
            _testClassPartialMock.Object.CurrentState.Should().Be(States.Stopped);
            _socketMock.Verify(socket => socket.DisconnectAsync(), Times.Once());
        }

        [TearDown]
        public void StockReceivingTask()
        {
            _testClassPartialMock.Object._currentState = States.Stopped;
            _socketMock.SetupGet(socket => socket.IsConnected).Returns(false);
        }
    }
}
