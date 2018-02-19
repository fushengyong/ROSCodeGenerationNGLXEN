namespace RosbridgeClientV2_0.UnitTests
{
    using Moq;
    using NUnit.Framework;
    using Rosbridge.Client.Common.Interfaces;

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
    }
}
