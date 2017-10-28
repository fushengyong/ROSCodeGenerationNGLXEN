namespace RosbridgeClientCommon.UnitTests
{
    using FluentAssertions;
    using Interfaces;
    using Moq;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class MessageDispatcherUnitTests
    {
        private MessageDispatcher _testClass;
        private Mock<ISocket> _socketMock;
        private Mock<IMessageSerializer> _messageSerializerMock;

        [SetUp]
        public void SetUp()
        {
            _socketMock = new Mock<ISocket>();
            _messageSerializerMock = new Mock<IMessageSerializer>();
            _testClass = new MessageDispatcher(_socketMock.Object, _messageSerializerMock.Object);
        }

        [Test]
        public void StartAsync_SocketThrowsException_MessageDispatcherShouldBeStopped()
        {
            //arrange
            _socketMock.Setup(socket => socket.ConnectAsync()).Throws<Exception>();

            //act
            _testClass.StartAsync();

            //assert
            _testClass.CurrentState.Should().Be(Enums.States.Stopped);
            _socketMock.Verify(socket => socket.ConnectAsync(), Times.Once());
            _socketMock.Verify(socket => socket.ReceiveAsync(), Times.Never());
        }

        [Test]
        public void StartAsync_SocketDidNotThrowException_MessageDispatcherShouldBeStarted()
        {
            //arrange

            //act
            _testClass.StartAsync();

            //assert
            _testClass.CurrentState.Should().Be(Enums.States.Started);
            _socketMock.Verify(socket => socket.ConnectAsync(), Times.Once());
        }
    }
}
