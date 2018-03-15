namespace Rosbridge.Client.Common.UnitTests
{
    using FluentAssertions;
    using Moq;
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

        [TearDown]
        public void StockReceivingTask()
        {
            _testClassPartialMock.Object._currentState = States.Stopped;
            _socketMock.SetupGet(socket => socket.IsConnected).Returns(false);
        }

        [Test]
        public void GetNewUniqueID_ResultShouldBeGuid()
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
        public void StartAsync_ClassDisposed_ShouldThrowObjectDisposedException()
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
        public void StartAsync_CurrentStateIsNotStopped_ShouldThrowMessageDispatcherException()
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
        public async Task StartAsync_DispatcherShouldStarted()
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
        public void StartAsync_SocketThrowsException_ShouldThrowInvalidOperationExceptionDispatcherShouldStopped()
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
        public void StopAsync_ObjectDisposed_ShouldThrowObjectDisposedException()
        {
            //arrange
            _testClassPartialMock.Object._disposed = true;

            //act
            Action act = () => _testClassPartialMock.Object.StopAsync();

            //assert
            act.Should().ThrowExactly<ObjectDisposedException>();
        }

        [Test]
        public void StopAsync_ObjectNotInStartedState_ShouldThrowMessageDispatcherException()
        {
            //arrange
            _testClassPartialMock.Object._currentState = States.Stopped;

            //act
            Action act = () => _testClassPartialMock.Object.StopAsync();

            //assert
            act.Should().ThrowExactly<MessageDispatcherException>();
        }

        [Test]
        public async Task StopAsync_ObjectShouldBeInStoppedState()
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

        [Test]
        public async Task StopAsync_ReceivingTaskNotNull_ReceivingTaskShouldBeDisposed()
        {
            //arrange
            _testClassPartialMock.Object._currentState = States.Started;
            _testClassPartialMock.Object._receivingTask = Task.Run(() => { });

            //act
            Task result = _testClassPartialMock.Object.StopAsync();

            //assert
            await result;
            result.Should().NotBeNull();
            _testClassPartialMock.Object._receivingTask.Should().BeNull();
        }

        [Test]
        public void SendAsync_ShouldCallSocketSendAsync()
        {
            //arrange
            object testMessage = new object();
            byte[] serializedMessage = new byte[1];

            _testClassPartialMock.Object._currentState = States.Started;
            _serializerMock.Setup(serializer => serializer.Serialize(testMessage)).Returns(serializedMessage);

            //act
            Task result = _testClassPartialMock.Object.SendAsync(testMessage);

            //assert
            result.Should().NotBeNull();

            _serializerMock.Verify(serializer => serializer.Serialize(testMessage), Times.Once);
            _socketMock.Verify(socket => socket.SendAsync(serializedMessage), Times.Once);
        }

        [Test]
        public void SendAsync_MessageIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            object testMessage = null;

            _testClassPartialMock.Object._currentState = States.Started;

            //act
            Action act = () => _testClassPartialMock.Object.SendAsync(testMessage);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void SendAsync_TestClassIsDisposed_ShouldThrowObjectDisposedException()
        {
            //arrange
            object testMessage = new object();

            _testClassPartialMock.Object._disposed = true;
            _testClassPartialMock.Object._currentState = States.Started;

            //act
            Action act = () => _testClassPartialMock.Object.SendAsync(testMessage);

            //assert
            act.Should().ThrowExactly<ObjectDisposedException>();
        }

        [Test]
        public void SendAsync_TestClassNotInStartedState_ShouldThrowMessageDispatcherException()
        {
            //arrange
            object testMessage = new object();

            _testClassPartialMock.Object._currentState = States.Stopped;

            //act
            Action act = () => _testClassPartialMock.Object.SendAsync(testMessage);

            //assert
            act.Should().ThrowExactly<MessageDispatcherException>();
        }

        [Test]
        public void Dispose_TestClassAlreadyDisposed_ShouldNotDoAnything()
        {
            //arrange
            _testClassPartialMock.Object._disposed = true;
            _testClassPartialMock.Object._currentState = States.Started;

            //act
            _testClassPartialMock.Object.Dispose();

            //assert
            _testClassPartialMock.Object._currentState.Should().Be(States.Started);

            _socketMock.Verify(socket => socket.DisconnectAsync(), Times.Never);
            _socketMock.Verify(socket => socket.Dispose(), Times.Never);
        }

        [Test]
        public async Task Dispose_TestClassNotDisposedYet_ShouldDispose()
        {
            //arrange
            _testClassPartialMock.Object._receivingTask = Task.Run(() => { });

            //act
            _testClassPartialMock.Object.Dispose();

            //assert
            await _testClassPartialMock.Object._disposingTask;
            _testClassPartialMock.Object._disposed.Should().BeTrue();
            _testClassPartialMock.Object._currentState.Should().Be(States.Stopped);
            _testClassPartialMock.Object._receivingTask.Should().BeNull();

            _socketMock.Verify(socket => socket.DisconnectAsync(), Times.Once);
            _socketMock.Verify(socket => socket.Dispose(), Times.Once);
        }
    }
}