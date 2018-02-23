namespace Rosbridge.Client.Common.UnitTests
{
    using FluentAssertions;
    using NUnit.Framework;
    using Rosbridge.Client.Common.Attributes;
    using Rosbridge.Client.Common.Exceptions;
    using Rosbridge.Client.Common.UnitTests.Utilities;
    using System;

    [TestFixture]
    public class RosMessageTypeAttributeHelperUnitTests
    {
        private RosMessageTypeAttributeHelper _testClass;
        private TypeBuilder _typeBuilder;

        [SetUp]
        public void SetUp()
        {
            _testClass = new RosMessageTypeAttributeHelper();
            _typeBuilder = new TypeBuilder("TestAssembly", System.Reflection.Emit.AssemblyBuilderAccess.Run, "TestModule");
        }

        [Test]
        public void GetRosMessageTypeFromTypeAttribute_UnitTest_TypeOk_ShouldReturnProperRosMessageTypeString()
        {
            //arrange
            string typeName = "testType";
            string rosMessageType = "testRosMessageType";

            _typeBuilder.Inicialize(typeName, System.Reflection.TypeAttributes.Public, null);
            _typeBuilder.AddCustomAttribute(typeof(RosMessageTypeAttribute), new Type[] { typeof(string) }, new object[] { rosMessageType });

            Type testType = _typeBuilder.CreateType();

            //act
            string resultRosMessageTypeString = _testClass.GetRosMessageTypeFromTypeAttribute(testType);

            //assert
            resultRosMessageTypeString.Should().NotBeNull();
            resultRosMessageTypeString.Should().Be(rosMessageType);
        }

        [Test]
        public void GetRosMessageTypeFromTypeAttribute_UnitTest_ArgumentNull_ShouldThrowArgumentNullException()
        {
            //arrange
            Type testType = null;

            //act
            Action act = () => _testClass.GetRosMessageTypeFromTypeAttribute(testType);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void GetRosMessageTypeFromTypeAttribute_UnitTest_NoRosMessageTypeAttributeOnType_ShouldThrowRosMessageTypeAttributeNullException()
        {
            //arrange
            string typeName = "testType";

            _typeBuilder.Inicialize(typeName, System.Reflection.TypeAttributes.Public, null);

            Type testType = _typeBuilder.CreateType();

            //act
            Action act = () => _testClass.GetRosMessageTypeFromTypeAttribute(testType);

            //assert
            act.Should().ThrowExactly<RosMessageTypeAttributeNullException>();
        }

        [Test]
        public void GetRosMessageTypeFromTypeAttribute_UnitTest_EmptyRosMessageType_ShouldThrowRosMessageTypeAttributeEmptyException()
        {
            //arrange
            string typeName = "testType";
            string rosMessageType = "";

            _typeBuilder.Inicialize(typeName, System.Reflection.TypeAttributes.Public, null);
            _typeBuilder.AddCustomAttribute(typeof(RosMessageTypeAttribute), new Type[] { typeof(string) }, new object[] { rosMessageType });

            Type testType = _typeBuilder.CreateType();

            //act
            Action act = () => _testClass.GetRosMessageTypeFromTypeAttribute(testType);

            //assert
            act.Should().ThrowExactly<RosMessageTypeAttributeEmptyException>();
        }
    }
}
