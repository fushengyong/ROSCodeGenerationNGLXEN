namespace RosbridgeClientV2_0.UnitTests
{
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using Rosbridge.Client.Common.Attributes;
    using Rosbridge.Client.Common.Exceptions;
    using Rosbridge.Client.Common.Interfaces;
    using Rosbridge.Client.V2_0;
    using Rosbridge.Client.V2_0.Messages.RosOperations;
    using System;
    using System.Linq;

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
        public void Constructor_DependenciesOk_FieldsShouldBeOk()
        {
            //arrange
            Type testMessageType = typeof(RosMessageTestClass);
            RosMessageTypeAttribute testClassAttribute = (RosMessageTypeAttribute)testMessageType.GetCustomAttributes(false).SingleOrDefault(attribute => attribute is RosMessageTypeAttribute);
            string topic = "TestTopic";

            //act
            Subscriber<RosMessageTestClass> testClass = new Subscriber<RosMessageTestClass>(topic, _messageDispatcherMock.Object, _rosMessageTypeAttributeHelperMock.Object);

            //assert
            testClass.Topic.Should().Be(topic);
            testClass.Type.Should().Be(testClassAttribute.RosMessageType);
        }

        [Test]
        public void Constructor_WrongType_ShouldThrowRosMessageTypeAttributeNullException()
        {
            //arrange
            string topic = "TestTopic";

            //act
            Action act = () => new Subscriber<object>(topic, _messageDispatcherMock.Object, _rosMessageTypeAttributeHelperMock.Object);

            //assert
            act.Should().Throw<RosMessageTypeAttributeNullException>();
        }

        [Test]
        public void SubscribeAsync_DispatcherSendAsyncMethodShouldCalledRosSubscribeMessage()
        {
            //arrange
            string topic = "TestTopic";
            Subscriber<RosMessageTestClass> testClass = new Subscriber<RosMessageTestClass>(topic, _messageDispatcherMock.Object, _rosMessageTypeAttributeHelperMock.Object);

            //act
            testClass.SubscribeAsync();

            //assert
            _messageDispatcherMock.Verify(dispatcher => dispatcher.SendAsync(It.IsAny<RosSubscribeMessage>()), Times.Once);
        }

        [Test]
        public void UnsubscribeAsync_DispatcherSendAsyncMethodShouldCalledWithRosUnsubscribeMessage()
        {
            //arrange
            string topic = "TestTopic";
            Subscriber<RosMessageTestClass> testClass = new Subscriber<RosMessageTestClass>(topic, _messageDispatcherMock.Object, _rosMessageTypeAttributeHelperMock.Object);

            //act
            testClass.UnsubscribeAsync();

            //assert
            _messageDispatcherMock.Verify(dispatcher => dispatcher.SendAsync(It.IsAny<RosUnsubscribeMessage>()), Times.Once);
        }
    }
}
