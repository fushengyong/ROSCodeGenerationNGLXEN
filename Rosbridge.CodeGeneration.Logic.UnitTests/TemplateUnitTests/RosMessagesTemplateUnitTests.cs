namespace Rosbridge.CodeGeneration.Logic.UnitTests.TemplateUnitTests
{
    using Microsoft.VisualStudio.TextTemplating;
    using NUnit.Framework;
    using Rosbridge.CodeGeneration.Logic.Constants;
    using Rosbridge.CodeGeneration.Logic.UnitTests.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using T4Template.Utilities.TemplateProcess;
    using T4Template.Utilities.TemplateProcess.Interfaces;

    [TestFixture]
    public class RosMessagesTemplateUnitTests
    {
        private const string TEMPLATE_RELATIVE_PATH_TO_EXECUTING_PROJECT_DIRECTORY = @"..\Rosbridge.CodeGeneration.Logic\Templates\CodeGenerators\RosMessage.tt";

        private FileInfo _template;
        private CustomTextTemplatingEngineHost _textTemplatingEngineHost;
        private ITemplateProcessor _templateProcessor;

        [SetUp]
        public void SetUp()
        {
            DirectoryInfo currentProjectDirectory = ExecutingProjectFinder.GetExecutingProjectDirectory();

            if (!currentProjectDirectory.Exists)
            {
                throw new DirectoryNotFoundException(currentProjectDirectory.FullName);
            }

            _template = new FileInfo(Path.Combine(currentProjectDirectory.FullName, TEMPLATE_RELATIVE_PATH_TO_EXECUTING_PROJECT_DIRECTORY));

            if (!_template.Exists)
            {
                throw new FileNotFoundException(_template.FullName);
            }

            _textTemplatingEngineHost = new CustomTextTemplatingEngineHost(true);
            _templateProcessor = new TemplateProcessor(_textTemplatingEngineHost);
        }

        [Test]
        public void Test1()
        {
            ITextTemplatingSession session = CreateTemplateHost(
                "testAttributeNamespace",
                "testAttributeType",
                "testNamespaceFrefix",
                "testNamespace",
                "testType",
                new string[] { "System", "System.IO" },
                Enumerable.Empty<Tuple<string, string, string>>(),
                Enumerable.Empty<Tuple<string, string, int>>(),
                new Dictionary<string, string>());

            string templateOutput = _templateProcessor.ProcessTemplateWithSession(_template, session);
        }

        private ITextTemplatingSession CreateTemplateHost(string messageTypeAttributeNamespace, string messageTypeAttributeName, string namespacePrefix, string @namespace, string type, IEnumerable<string> dependencyList, IEnumerable<Tuple<string, string, string>> constantFieldList, IEnumerable<Tuple<string, string, int>> arrayFieldList, IDictionary<string, string> fieldList)
        {
            ITextTemplatingSession session = new TextTemplatingSession();

            session[TemplateParameterConstants.RosMessage.ROS_MESSAGE_TYPE_ATTRIBUTE_NAMESPACE] = messageTypeAttributeNamespace;
            session[TemplateParameterConstants.RosMessage.ROS_MESSAGE_TYPE_ATTRIBUTE_NAME] = messageTypeAttributeName;
            session[TemplateParameterConstants.RosMessage.NAMESPACE_PREFIX] = namespacePrefix;
            session[TemplateParameterConstants.RosMessage.NAMESPACE] = @namespace;
            session[TemplateParameterConstants.RosMessage.TYPE] = type;
            session[TemplateParameterConstants.RosMessage.DEPENDENCY_LIST] = dependencyList;
            session[TemplateParameterConstants.RosMessage.CONSTANT_FIELD_LIST] = constantFieldList;
            session[TemplateParameterConstants.RosMessage.ARRAY_FIELD_LIST] = arrayFieldList;
            session[TemplateParameterConstants.RosMessage.FIELD_LIST] = fieldList;

            return session;
        }
    }
}
