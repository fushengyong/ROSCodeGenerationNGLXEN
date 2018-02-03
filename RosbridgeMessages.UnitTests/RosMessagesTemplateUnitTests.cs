namespace RosbridgeMessages.UnitTests
{
    using FluentAssertions;
    using Microsoft.VisualStudio.TextTemplating;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.IO;

    [TestFixture]
    public class RosMessagesTemplateUnitTests
    {
        private const string TEST_TEMPLATE_RELATIVE_PATH = @"RosbridgeMessages\Templates\CodeGenerators\RosMessage.tt";
        private const string MESSAGE_TYPE_ATTRIBUTE_NAME_PARAMETER_NAME = "MessageTypeAttributeName";
        private const string MESSAGE_TYPE_ATTRIBUTE_NAMESPACE_PARAMETER_NAME = "MessageTypeAttributeNamespace";
        private const string NAMESPACE_PREFIX_PARAMETER_NAME = "NamespacePrefix";
        private const string DEPENDENCY_LIST_PARAMETER_NAME = "DependencyList";
        private const string MESSAGE_NAMESPACE_PARAMETER_NAME = "MessageNamespace";
        private const string MESSAGE_TYPE_PARAMETER_NAME = "MessageType";
        private const string CONSTANT_FIELD_LIST_PARAMETER_NAME = "ConstantFieldList";
        private const string ARRAY_FIELD_LIST_PARAMETER_NAME = "ArrayFieldList";
        private const string FIELD_LIST_PARAMETER_NAME = "FieldList";

        private FileInfo _testTemplate;

        [SetUp]
        public void SetUp()
        {
            DirectoryInfo solutionDirectory = new DirectoryInfo(new Uri(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)).LocalPath);

            while (solutionDirectory != null && solutionDirectory.GetFiles("*.sln", SearchOption.TopDirectoryOnly).Length == 0)
            {
                solutionDirectory = solutionDirectory.Parent;
            }

            _testTemplate = new FileInfo(Path.Combine(solutionDirectory.FullName, TEST_TEMPLATE_RELATIVE_PATH));
            _testTemplate.Exists.Should().BeTrue();
        }

        private TextTemplatingSession CreateSessionWithParameters(string messageTypeAttributeName, string messageTypeAttributeNamespace, string namespacePrefix, List<string> dependencyList, string messageNamespace, string messageType, List<Tuple<string, string, string>> constantFieldList, List<Tuple<string, string, int>> arrayFieldList, Dictionary<string, string> fieldList)
        {
            TextTemplatingSession session = new TextTemplatingSession();
            session[MESSAGE_TYPE_ATTRIBUTE_NAME_PARAMETER_NAME] = messageTypeAttributeName;
            session[MESSAGE_TYPE_ATTRIBUTE_NAMESPACE_PARAMETER_NAME] = messageTypeAttributeNamespace;
            session[NAMESPACE_PREFIX_PARAMETER_NAME] = namespacePrefix;
            session[DEPENDENCY_LIST_PARAMETER_NAME] = dependencyList;
            session[MESSAGE_NAMESPACE_PARAMETER_NAME] = messageNamespace;
            session[MESSAGE_TYPE_PARAMETER_NAME] = messageType;
            session[CONSTANT_FIELD_LIST_PARAMETER_NAME] = constantFieldList;
            session[ARRAY_FIELD_LIST_PARAMETER_NAME] = arrayFieldList;
            session[FIELD_LIST_PARAMETER_NAME] = fieldList;
            return session;
        }

        [Test]
        public void Test()
        {
            //arrange
            Engine engine = new Engine();
            CustomCmdLineHost customHost = new CustomCmdLineHost(_testTemplate.FullName);
            customHost.Session = CreateSessionWithParameters(
                "RosMessageTypeAttribute",
                "RosMessageNamespace",
                "DefaultNamespace",
                new List<string>(),
                "TestNamespace",
                "TestClass",
                new List<Tuple<string, string, string>>(),
                new List<Tuple<string, string, int>>(),
                new Dictionary<string, string>());

            //act
            string transformedText = engine.ProcessTemplate(File.ReadAllText(_testTemplate.FullName), customHost);
            //assert
            customHost.Errors.Should().BeEmpty();

        }
    }
}
