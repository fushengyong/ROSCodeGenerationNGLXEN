namespace Rosbridge.CodeGeneration.Logic.UnitTests.LogicUnitTests
{
    using BaseClasses;
    using FluentAssertions;
    using Helpers;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;

    [TestFixture]
    public class YAMLParserUnitTests
    {
        private Mock<YAMLParser> _testClassPartialMock;
        private IDictionary<string, string> _primititveTypeDictionary;

        [SetUp]
        public void SetUp()
        {
            _primititveTypeDictionary = new Dictionary<string, string>();
            _testClassPartialMock = new Mock<YAMLParser>(_primititveTypeDictionary);
        }

        [Test]
        public void Constructor_FieldsShouldSetCorrectly()
        {
            //arrange
            IDictionary<string, string> primitiveTypeDictionary = new Dictionary<string, string>();

            //act
            YAMLParser testClass = new YAMLParser(primitiveTypeDictionary);

            //assert
            testClass._primitiveTypeDictionary.Should().NotBeNull();
        }

        [Test]
        public void Constructor_PrimitiveTypeDictionaryArgumentIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            IDictionary<string, string> primitiveTypeDictionary = null;

            //act
            Action act = () => new YAMLParser(primitiveTypeDictionary);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void SetMsgFileFieldFromYAMLString_YamlStringIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            string yamlString = null;

            //act
            Action act = () => _testClassPartialMock.Object.SetMsgFileFieldsFromYAMLString(yamlString, It.IsAny<MsgFile>());

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void SetMsgFileFieldFromYAMLString_MsgFileIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            string yamlString = "testYamlString";

            //act
            Action act = () => _testClassPartialMock.Object.SetMsgFileFieldsFromYAMLString(yamlString, null);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}
