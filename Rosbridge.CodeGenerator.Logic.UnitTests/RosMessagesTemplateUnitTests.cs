namespace Rosbridge.CodeGenerator.Logic.UnitTests
{
    using Microsoft.VisualStudio.TextTemplating;
    using Microsoft.VisualStudio.TextTemplating.VSHost;
    using NUnit.Framework;
    using Rosbridge.CodeGenerator.Logic.Helpers;
    using Rosbridge.CodeGenerator.Logic.UnitTests.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    [TestFixture]
    public class RosMessagesTemplateUnitTests
    {
        private const string TEMPLATE_RELATIVE_PATH = @"..\Rosbridge.CodeGenerator.Logic\Templates\CodeGenerators\RosMessage.tt";

        private FileInfo _template;
        private TemplateProcessor _templateProcessor;

        [SetUp]
        public void SetUp()
        {
            DirectoryInfo currentProjectDirectory = ExecutingProjectFinder.GetExecutingProjectDirectory();

            if (!currentProjectDirectory.Exists)
            {
                throw new DirectoryNotFoundException(currentProjectDirectory.FullName);
            }

            _template = new FileInfo(Path.Combine(currentProjectDirectory.FullName, TEMPLATE_RELATIVE_PATH));

            if (!_template.Exists)
            {
                throw new FileNotFoundException(_template.FullName);
            }

            IServiceProvider serviceProvider = ServiceProviderGetter.GetServiceProvider("VisualStudio.DTE.15.0");

            ITextTemplatingEngineHost textTemplatingEngineHost = (ITextTemplatingEngineHost)serviceProvider.GetService(typeof(ITextTemplatingEngineHost));
            ITextTemplating textTemplating = (ITextTemplating)serviceProvider.GetService(typeof(ITextTemplating));
            ITextTemplatingSessionHost textTemplatingSessionHost = (ITextTemplatingSessionHost)serviceProvider.GetService(typeof(ITextTemplatingSessionHost));

            _templateProcessor = new TemplateProcessor(textTemplatingEngineHost, textTemplating, textTemplatingSessionHost);
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

            session[CodeGenerator.ROS_MESSAGE_TEMPLATE_PARAMETER_MESSAGE_TYPE_ATTRIBUTE_NAMESPACE] = messageTypeAttributeNamespace;
            session[CodeGenerator.ROS_MESSAGE_TEMPLATE_PARAMETER_MESSAGE_TYPE_ATTRIBUTE_NAME] = messageTypeAttributeName;
            session[CodeGenerator.ROS_MESSAGE_TEMPLATE_PARAMETER_NAMESPACE_PREFIX] = namespacePrefix;
            session[CodeGenerator.TEMPLATE_PARAMETER_NAMESPACE] = @namespace;
            session[CodeGenerator.TEMPLATE_PARAMETER_TYPE] = type;
            session[CodeGenerator.ROS_MESSAGE_TEMPLATE_PARAMETER_DEPENDENCY_LIST] = dependencyList;
            session[CodeGenerator.ROS_MESSAGE_TEMPLATE_PARAMETER_CONSTANT_FIELD_LIST] = constantFieldList;
            session[CodeGenerator.ROS_MESSAGE_TEMPLATE_PARAMETER_ARRAY_FIELD_LIST] = arrayFieldList;
            session[CodeGenerator.ROS_MESSAGE_TEMPLATE_PARAMETER_FIELD_LIST] = fieldList;

            return session;
        }
    }
}
