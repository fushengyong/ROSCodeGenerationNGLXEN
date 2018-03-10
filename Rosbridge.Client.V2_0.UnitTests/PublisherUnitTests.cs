namespace Rosbridge.Client.V2_0.UnitTests
{
    using Common.Interfaces;
    using FluentAssertions;
    using Messages.RosOperations;
    using Moq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using System;
    using System.Threading.Tasks;

    [TestFixture]
    public class PublisherUnitTests
    {
        private Mock<IMessageDispatcher> _messageDispatcherMock;
        private Mock<IRosMessageTypeAttributeHelper> _rosMessageAttributeHelper;

        [SetUp]
        public void SetUp()
        {
            _messageDispatcherMock = new Mock<IMessageDispatcher>();
            _rosMessageAttributeHelper = new Mock<IRosMessageTypeAttributeHelper>();
        }

        [Test]
        public void Constructor_ShouldSetFields()
        {
            //arrange
            string testTopic = "testTopic";
            string testType = "testType";
            string uniqueId = Guid.NewGuid().ToString();

            _messageDispatcherMock.Setup(dispatcher => dispatcher.GetNewUniqueID()).Returns(uniqueId);
            _rosMessageAttributeHelper.Setup(helper => helper.GetRosMessageTypeFromTypeAttribute(It.IsAny<Type>()))
                .Returns(testType);

            //act
            Publisher<object> testClass = new Publisher<object>(testTopic, _messageDispatcherMock.Object, _rosMessageAttributeHelper.Object);

            //assert
            testClass.Topic.Should().Be(testTopic);
            testClass.Type.Should().Be(testType);
            testClass._uniqueId.Should().Be(uniqueId);
        }

        [Test]
        public void Constructor_TopicIsNull_ShouldThrowArgumentException()
        {
            //arrange
            string testTopic = null;

            //act
            Action act = () => new Publisher<object>(testTopic, _messageDispatcherMock.Object, _rosMessageAttributeHelper.Object);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void Constructor_TopicIsEmpty_ShouldThrowArgumentException()
        {
            //arrange
            string testTopic = string.Empty;

            //act
            Action act = () => new Publisher<object>(testTopic, _messageDispatcherMock.Object, _rosMessageAttributeHelper.Object);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void Constructor_MessageDispatcherIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            string testTopic = "testTopic";

            //act
            Action act = () => new Publisher<object>(testTopic, null, _rosMessageAttributeHelper.Object);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void Constructor_RosMessageTypeAttributeHelperIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            string testTopic = "testTopic";

            //act
            Action act = () => new Publisher<object>(testTopic, _messageDispatcherMock.Object, null);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void AdvertiseAsync_MessageDispatcherSendAsyncShouldCalledWithAppropriateObject()
        {
            //arrange
            string testTopic = "testTopic";
            string testType = "testType";
            string uniqueId = Guid.NewGuid().ToString();
            Task resultTask = Task.Run(() => { });

            _messageDispatcherMock.Setup(dispatcher => dispatcher.GetNewUniqueID()).Returns(uniqueId);
            _rosMessageAttributeHelper.Setup(helper => helper.GetRosMessageTypeFromTypeAttribute(typeof(object)))
                .Returns(testType);
            _messageDispatcherMock.Setup(dispatcher => dispatcher.SendAsync(It.IsAny<RosAdvertiseMessage>())).Returns(resultTask);

            Publisher<object> testClass = new Publisher<object>(testTopic, _messageDispatcherMock.Object, _rosMessageAttributeHelper.Object);

            //act
            Task result = testClass.AdvertiseAsync();

            //assert
            result.Should().NotBeNull();
            result.Should().Be(resultTask);
            _messageDispatcherMock.Verify(dispatcher => dispatcher.SendAsync(It.Is<RosAdvertiseMessage>(message =>
                message.Id == uniqueId &&
                message.Topic == testTopic &&
                message.Type == testType)), Times.Once);
        }

        [Test]
        public void UnadvertiseAsync_MessageDispatcherSendAsyncShouldCalledWithAppropriateObject()
        {
            //arrange
            string testTopic = "testTopic";
            string testType = "testType";
            string uniqueId = Guid.NewGuid().ToString();
            Task resultTask = Task.Run(() => { });

            _messageDispatcherMock.Setup(dispatcher => dispatcher.GetNewUniqueID()).Returns(uniqueId);
            _rosMessageAttributeHelper.Setup(helper => helper.GetRosMessageTypeFromTypeAttribute(typeof(object)))
                .Returns(testType);
            _messageDispatcherMock.Setup(dispatcher => dispatcher.SendAsync(It.IsAny<RosUnadvertiseMessage>())).Returns(resultTask);

            Publisher<object> testClass = new Publisher<object>(testTopic, _messageDispatcherMock.Object, _rosMessageAttributeHelper.Object);

            //act
            Task result = testClass.UnadvertiseAsync();

            //assert
            result.Should().NotBeNull();
            result.Should().Be(resultTask);
            _messageDispatcherMock.Verify(dispatcher => dispatcher.SendAsync(It.Is<RosUnadvertiseMessage>(message =>
                message.Id == uniqueId &&
                message.Topic == testTopic)), Times.Once);
        }

        [Test]
        public void PublishAsync_MessageDispatcherShouldSendAppropriateObject()
        {
            //arrange
            string testTopic = "testTopic";
            string testType = "testType";
            string uniqueId = Guid.NewGuid().ToString();
            object messageObject = new { Test = "test", Property = "Prop" };
            JObject messageJSONObject = JObject.FromObject(messageObject);
            Task resultTask = Task.Run(() => { });

            _messageDispatcherMock.Setup(dispatcher => dispatcher.GetNewUniqueID()).Returns(uniqueId);
            _rosMessageAttributeHelper.Setup(helper => helper.GetRosMessageTypeFromTypeAttribute(It.IsAny<Type>()))
                .Returns(testType);
            _messageDispatcherMock.Setup(dispatcher => dispatcher.SendAsync(It.IsAny<RosPublishMessage>())).Returns(resultTask);

            Publisher<object> testClass = new Publisher<object>(testTopic, _messageDispatcherMock.Object, _rosMessageAttributeHelper.Object);

            //act
            Task result = testClass.PublishAsync(messageObject);

            //assert
            result.Should().NotBeNull();
            result.Should().Be(resultTask);
            _messageDispatcherMock.Verify(dispatcher => dispatcher.SendAsync(It.Is<RosPublishMessage>(message =>
                message.Id == uniqueId &&
                message.Topic == testTopic &&
                JToken.DeepEquals(message.Message, messageJSONObject))), Times.Once);
        }

        [Test]
        public void PublishAsync_MessageIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            string testTopic = "testTopic";
            string testType = "testType";
            string uniqueId = Guid.NewGuid().ToString();

            _messageDispatcherMock.Setup(dispatcher => dispatcher.GetNewUniqueID()).Returns(uniqueId);
            _rosMessageAttributeHelper.Setup(helper => helper.GetRosMessageTypeFromTypeAttribute(It.IsAny<Type>()))
                .Returns(testType);

            Publisher<object> testClass = new Publisher<object>(testTopic, _messageDispatcherMock.Object, _rosMessageAttributeHelper.Object);

            //act
            Action act = () => testClass.PublishAsync(null);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}