namespace Rosbridge.Client.V2_0.UnitTests
{
    using Common.Interfaces;
    using FluentAssertions;
    using Moq;
    using NUnit.Framework;
    using System;

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
        public void Constructor_UnitTest_ArgumentsOK_ParametersOK()
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
    }
}
