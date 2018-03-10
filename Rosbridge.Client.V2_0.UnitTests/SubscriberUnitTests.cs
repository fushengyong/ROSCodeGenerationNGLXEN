using FluentAssertions;
using Rosbridge.Client.V2_0.Messages.RosOperations;
using System;

namespace Rosbridge.Client.V2_0.UnitTests
{
    using Moq;
    using NUnit.Framework;
    using Rosbridge.Client.Common.Interfaces;
    using System.Threading.Tasks;

    [TestFixture]
    public class SubscriberUnitTests
    {
        private Mock<IMessageDispatcher> _messageDispatcherMock;
        private Mock<IRosMessageTypeAttributeHelper> _rosMessageTypeAttributeHelperMock;

        [SetUp]
        public void SetUp()
        {
            _messageDispatcherMock = new Mock<IMessageDispatcher>();
            _rosMessageTypeAttributeHelperMock = new Mock<IRosMessageTypeAttributeHelper>();
        }

        [Test]
        public void Constructor_ShouldSetFields()
        {
            //arrange
            string testTopic = "testTopic";
            string testRosMessageType = "testType";
            string uniqueId = Guid.NewGuid().ToString();


            _messageDispatcherMock.Setup(messageDispatcher => messageDispatcher.GetNewUniqueID()).Returns(uniqueId);
            _rosMessageTypeAttributeHelperMock.Setup(attributeHelper =>
                attributeHelper.GetRosMessageTypeFromTypeAttribute(It.IsAny<Type>())).Returns(testRosMessageType);

            //act
            RosSubscriber<object> testClass = new RosSubscriber<object>(testTopic, _messageDispatcherMock.Object, _rosMessageTypeAttributeHelperMock.Object);

            //assert
            testClass._uniqueId.Should().Be(uniqueId);
            testClass.Topic.Should().Be(testTopic);
            testClass.Type.Should().Be(testRosMessageType);
        }

        [Test]
        public void Constructor_TopicIsNull_ShouldThrowArgumentException()
        {
            //arrange
            string testTopic = null;

            //act
            Action act = () => new RosSubscriber<object>(testTopic, _messageDispatcherMock.Object, _rosMessageTypeAttributeHelperMock.Object);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void Constructor_TopicIsEmpty_ShouldThrowArgumentException()
        {
            //arrange
            string testTopic = string.Empty;

            //act
            Action act = () => new RosSubscriber<object>(testTopic, _messageDispatcherMock.Object, _rosMessageTypeAttributeHelperMock.Object);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void Constructor_MessageDispatcherIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            string testTopic = "testTopic";

            //act
            Action act = () => new RosSubscriber<object>(testTopic, null, _rosMessageTypeAttributeHelperMock.Object);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void Constructor_RosMessageTypeAttributeHelperIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            string testTopic = "testTopic";

            //act
            Action act = () => new RosSubscriber<object>(testTopic, _messageDispatcherMock.Object, null);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void SubscribeAsync_MessageDispatcherSendAsyncShouldCalledWithAppropriateObject()
        {
            //arrange
            string testTopic = "testTopic";
            string testRosMessageType = "testType";
            string uniqueId = Guid.NewGuid().ToString();
            Task resultTask = Task.Run(() => { });


            _messageDispatcherMock.Setup(messageDispatcher => messageDispatcher.GetNewUniqueID()).Returns(uniqueId);
            _messageDispatcherMock.Setup(dispatcher => dispatcher.SendAsync(It.IsAny<RosSubscribeMessage>()))
                .Returns(resultTask);
            _rosMessageTypeAttributeHelperMock.Setup(attributeHelper =>
                attributeHelper.GetRosMessageTypeFromTypeAttribute(typeof(object))).Returns(testRosMessageType);

            RosSubscriber<object> testClass = new RosSubscriber<object>(testTopic, _messageDispatcherMock.Object, _rosMessageTypeAttributeHelperMock.Object);


            //act
            Task result = testClass.SubscribeAsync();

            //assert
            result.Should().NotBeNull();
            result.Should().Be(resultTask);
            _messageDispatcherMock.Verify(dispatcher => dispatcher.SendAsync(It.Is<RosSubscribeMessage>(message =>
                message.Id == uniqueId &&
                message.Topic == testTopic &&
                message.Type == testRosMessageType)), Times.Once);
        }

        [Test]
        public void UnsubscribeAsync_MessageDispatcherSendAsyncShouldCalledWithAppropriateObject()
        {
            //arrange
            string testTopic = "testTopic";
            string testRosMessageType = "testType";
            string uniqueId = Guid.NewGuid().ToString();
            Task resultTask = Task.Run(() => { });


            _messageDispatcherMock.Setup(messageDispatcher => messageDispatcher.GetNewUniqueID()).Returns(uniqueId);
            _messageDispatcherMock.Setup(dispatcher => dispatcher.SendAsync(It.IsAny<RosUnsubscribeMessage>()))
                .Returns(resultTask);
            _rosMessageTypeAttributeHelperMock.Setup(attributeHelper =>
                attributeHelper.GetRosMessageTypeFromTypeAttribute(typeof(object))).Returns(testRosMessageType);

            RosSubscriber<object> testClass = new RosSubscriber<object>(testTopic, _messageDispatcherMock.Object, _rosMessageTypeAttributeHelperMock.Object);


            //act
            Task result = testClass.UnsubscribeAsync();

            //assert
            result.Should().NotBeNull();
            result.Should().Be(resultTask);
            _messageDispatcherMock.Verify(dispatcher => dispatcher.SendAsync(It.Is<RosUnsubscribeMessage>(message =>
                message.Id == uniqueId &&
                message.Topic == testTopic)), Times.Once);
        }
    }
}