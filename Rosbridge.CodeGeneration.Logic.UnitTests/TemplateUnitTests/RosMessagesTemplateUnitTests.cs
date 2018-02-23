namespace Rosbridge.CodeGeneration.Logic.UnitTests.TemplateUnitTests
{
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.VisualStudio.TextTemplating;
    using NUnit.Framework;
    using Rosbridge.CodeGeneration.Logic.Constants;
    using Rosbridge.CodeGeneration.Logic.UnitTests.Utilities;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using T4Template.Utilities.Interfaces;
    using T4Template.Utilities.TemplateCompile;
    using T4Template.Utilities.TemplateProcess;

    [TestFixture]
    public class RosMessagesTemplateUnitTests
    {
        private const string TEMPLATE_RELATIVE_PATH_TO_EXECUTING_PROJECT_DIRECTORY = @"..\Rosbridge.CodeGeneration.Logic\Templates\CodeGenerators\RosMessage.tt";

        private static readonly IEnumerable<string> DefaultNamespaces =
        new[]
        {
                    "System",
                    "System.IO",
                    "System.Net",
                    "System.Linq",
        };
        private static readonly CSharpCompilationOptions DefaultCompilationOptions =
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOverflowChecks(true)
                .WithPlatform(Platform.X86)
                .WithOptimizationLevel(OptimizationLevel.Release)
                .WithUsings(DefaultNamespaces);
        private static readonly IEnumerable<MetadataReference> DefaultReferences =
        new[]
        {
                    MetadataReference.CreateFromFile(typeof (object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof (System.Linq.Enumerable).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof (System.GenericUriParser).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).Assembly.Location)
        };

        private FileInfo _template;
        private ICustomTextTemplatingEngineHost _textTemplatingEngineHost;
        private ITemplateProcessor _templateProcessor;
        private ITemplateCompiler _templateCompiler;

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

            _textTemplatingEngineHost = new CustomTextTemplatingEngineHost(AppDomain.CurrentDomain);
            _templateProcessor = new TemplateProcessor(_textTemplatingEngineHost);
            _templateCompiler = new TemplateCompiler();
        }

        [Test]
        public void RosMessagesTemplate_UnitTest_ParametersOK_TemplateCreatesAppropriateOutput()
        {
            //arrange
            string rosbridgeAttributeNamespace = "System";
            string rosbridgeAttributeType = "testRosType";
            string namespacePrefix = "testPrefix";
            string testNamespace = "testNamespace";
            string testType = "testType";
            string[] testDependencies = new string[] { };
            IEnumerable<Tuple<string, string, string>> testConstantFields = new List<Tuple<string, string, string>>() { Tuple.Create("TestType", "TestName", "TestValue") };
            IEnumerable<Tuple<string, string, int>> testArrayFields = new List<Tuple<string, string, int>>() { Tuple.Create("TestType", "TestName", 2) };
            IDictionary<string, string> testFields = new Dictionary<string, string>()
            {
                { "TestFieldName", "TestFieldValue" }
            };

            ITextTemplatingSession session = CreateTemplateSession(
                rosbridgeAttributeNamespace,
                rosbridgeAttributeType,
                namespacePrefix,
                testNamespace,
                testType,
                testDependencies,
                testConstantFields,
                testArrayFields,
                testFields);

            //act
            string templateOutput = _templateProcessor.ProcessTemplateWithSession(_template, session);

            //assert
            SyntaxTree parsedTemplateOutput = _templateCompiler.ParseTemplateOutput(templateOutput);
            Assembly compiledAssembly = _templateCompiler.CompileSyntaxTree(parsedTemplateOutput, DefaultCompilationOptions, DefaultReferences, MethodBase.GetCurrentMethod().Name);
            compiledAssembly.Should().NotBeNull();
            compiledAssembly.DefinedTypes.Should().NotBeNull();
            compiledAssembly.DefinedTypes.Should().HaveCount(1);
            Type resultType = compiledAssembly.DefinedTypes.First();
            resultType.Name.Should().Be(testType);
            resultType.Namespace.Should().Be($"{namespacePrefix}.{testNamespace}");
            resultType.IsValueType.Should().BeFalse();
        }

        private ITextTemplatingSession CreateTemplateSession(string messageTypeAttributeNamespace, string messageTypeAttributeName, string namespacePrefix, string @namespace, string type, IEnumerable<string> dependencyList, IEnumerable<Tuple<string, string, string>> constantFieldList, IEnumerable<Tuple<string, string, int>> arrayFieldList, IDictionary<string, string> fieldList)
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
