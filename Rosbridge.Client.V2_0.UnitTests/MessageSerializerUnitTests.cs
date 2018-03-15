namespace Rosbridge.Client.V2_0.UnitTests
{
    using FluentAssertions;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using System;
    using System.Text;

    [TestFixture]
    public class MessageSerializerUnitTests
    {
        private MessageSerializer _testClass;

        [SetUp]
        public void SetUp()
        {
            _testClass = new MessageSerializer();
        }

        [Test]
        public void Deserialize_ShouldReturnAppropriateJObject()
        {
            //arrange
            object message = new { Test = "test", Property = "property" };
            string jsonString = JsonConvert.SerializeObject(message);
            byte[] byteArray = Encoding.ASCII.GetBytes(jsonString);
            JObject messageJObject = JObject.Parse(jsonString);

            //act
            JObject resultJObject = _testClass.Deserialize(byteArray);

            //assert
            resultJObject.Should().NotBeNull();
            JToken.DeepEquals(resultJObject, messageJObject).Should().BeTrue();
        }

        [Test]
        public void Deserialize_ByteArrayArgumentIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            byte[] buffer = null;

            //act
            Action act = () => _testClass.Deserialize(buffer);

            //asser
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void Serialize_ShouldReturnAppropriateByteArray()
        {
            //arrange
            object messageObject = new { Test = "test", Property = "property" };
            string jsonString = JsonConvert.SerializeObject(messageObject);
            JObject jObject = JObject.Parse(jsonString);

            //act
            byte[] resultByteArray = _testClass.Serialize(messageObject);
            string resultJsonString = Encoding.ASCII.GetString(resultByteArray, 0, resultByteArray.Length);
            JObject resultJObject = JObject.Parse(resultJsonString);

            //assert
            resultByteArray.Should().NotBeNull();
            JToken.DeepEquals(resultJObject, jObject);
        }

        [Test]
        public void Serialize_ObjectArgumentIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            object messageObject = null;

            //act
            Action act = () => _testClass.Serialize(messageObject);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}