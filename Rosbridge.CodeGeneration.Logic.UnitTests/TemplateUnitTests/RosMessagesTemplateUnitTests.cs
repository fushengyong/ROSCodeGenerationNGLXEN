namespace Rosbridge.CodeGeneration.Logic.UnitTests.TemplateUnitTests
{
    using Common.Testing.Utilities.Extensions;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.VisualStudio.TextTemplating;
    using NUnit.Framework;
    using Rosbridge.Client.Common.Attributes;
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
                    "System.Linq",
        };
        private static readonly CSharpCompilationOptions DefaultCompilationOptions =
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
                .WithOverflowChecks(true)
                .WithPlatform(Platform.X86)
                .WithUsings(DefaultNamespaces);
        private static readonly IEnumerable<MetadataReference> DefaultReferences =
        new[]
        {
                    MetadataReference.CreateFromFile(typeof (object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof (System.Linq.Enumerable).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof (System.GenericUriParser).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException).Assembly.Location),
                    MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0").Location),
                    MetadataReference.CreateFromFile(typeof (RosMessageTypeAttribute).Assembly.Location),
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
        public void RosMessagesTemplate_UnitTest_TpyeInfosConstantFieldInfosArrayAndNormalPropertyInfosSetAsSessionParameters_TemplateCreatesAppropriateTypeWithConstantFieldsArrayAndNormalProperties()
        {
            //arrange
            Type rosMessageTypeAttributeType = typeof(RosMessageTypeAttribute);
            string rosbridgeAttributeNamespace = rosMessageTypeAttributeType.Namespace;
            string rosbridgeAttributeType = rosMessageTypeAttributeType.Name;
            string namespacePrefix = "testPrefix";
            string testNamespace = "testNamespace";
            string testType = "testType";
            IEnumerable<string> testDependencies = new string[] { };
            IEnumerable<Tuple<string, string, string>> testConstantFields = new List<Tuple<string, string, string>>() { Tuple.Create("String", "TestFieldName1", "TestValue") };
            IEnumerable<Tuple<string, string, int>> testArrayFields = new List<Tuple<string, string, int>>() { Tuple.Create("String", "TestFieldName2", 2) };
            IDictionary<string, string> testFields = new Dictionary<string, string>()
            {
                { "TestFieldName3", "String" }
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
            _templateCompiler.CompilationInfo.Success.Should().BeTrue();

            compiledAssembly.Should().NotBeNull();
            compiledAssembly.DefinedTypes.Should().NotBeNull();
            compiledAssembly.DefinedTypes.Should().HaveCount(1);

            //type check
            Type resultType = compiledAssembly.DefinedTypes.First();
            resultType.Name.Should().Be(testType);
            resultType.Namespace.Should().Be($"{namespacePrefix}.{testNamespace}");
            resultType.IsValueType.Should().BeFalse();
            resultType.CustomAttributes.Should().Contain(attribute => attribute.AttributeType == rosMessageTypeAttributeType);

            MethodInfo equalsMethod = resultType.GetMethod("Equals");
            equalsMethod.Should().NotBeNull();
            equalsMethod.ReturnType.Should().Be(typeof(Boolean));
            equalsMethod.IsPublic.Should().BeTrue();
            equalsMethod.IsVirtual.Should().BeTrue();

            IEnumerable<FieldInfo> resultConstantFieldCollection = resultType.GetFields().Where(field => field.IsLiteral && !field.IsInitOnly).ToList();
            resultConstantFieldCollection.Should().NotBeNull();
            resultConstantFieldCollection.Should().HaveCount(1);

            Tuple<string, string, string> testConstantField = testConstantFields.First();
            FieldInfo resultConstantField = resultConstantFieldCollection.SingleOrDefault(field => field.Name == testConstantField.Item2);
            resultConstantField.Should().NotBeNull();
            resultConstantField.FieldType.Name.Should().Be(testConstantField.Item1);

            IEnumerable<PropertyInfo> resultPropertyCollection = resultType.GetProperties();
            resultPropertyCollection.Should().NotBeNull();
            resultPropertyCollection.Should().HaveCount(testArrayFields.Count() + testFields.Count);

            Tuple<string, string, int> testArrayField = testArrayFields.First();
            PropertyInfo resultArrayProperty = resultPropertyCollection.SingleOrDefault(property => property.Name == testArrayField.Item2);
            resultArrayProperty.Should().NotBeNull();
            resultArrayProperty.PropertyType.IsArray.Should().BeTrue();
            resultArrayProperty.PropertyType.GetElementType().Name.Should().Be(testArrayField.Item1);

            KeyValuePair<string, string> testField = testFields.FirstOrDefault();
            PropertyInfo resultFieldProperty = resultPropertyCollection.SingleOrDefault(property => property.Name == testField.Key);
            resultFieldProperty.Should().NotBeNull();
            resultFieldProperty.PropertyType.Name.Should().Be(testField.Value);

            //instance check
            dynamic instantiatedResultType = Activator.CreateInstance(resultType);

            dynamic resultConstantFieldInstanceValue = resultConstantField.GetValue(instantiatedResultType);
            testConstantField.Item3.Should().Be(resultConstantFieldInstanceValue);

            dynamic resultArrayPropertyInstanceValue = resultArrayProperty.GetValue(instantiatedResultType);
            testArrayField.Item3.Should().Be(resultArrayPropertyInstanceValue.Length);
        }

        [Test]
        public void RosMessagesTemplate_UnitTest_TypeInfoWithoutConstantFieldInfoOrPropertyInfo_TemplateCreatesAppropriateTypeWithoutConstantFieldsOrProperties()
        {
            //arrange
            Type rosMessageTypeAttributeType = typeof(RosMessageTypeAttribute);
            string rosbridgeAttributeNamespace = rosMessageTypeAttributeType.Namespace;
            string rosbridgeAttributeType = rosMessageTypeAttributeType.Name;
            string namespacePrefix = "testPrefix";
            string testNamespace = "testNamespace";
            string testType = "testType";
            IEnumerable<string> testDependencies = new string[] { };
            IEnumerable<Tuple<string, string, string>> testConstantFields = new List<Tuple<string, string, string>>();
            IEnumerable<Tuple<string, string, int>> testArrayFields = new List<Tuple<string, string, int>>();
            IDictionary<string, string> testFields = new Dictionary<string, string>();

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

            _templateCompiler.CompilationInfo.Success.Should().BeTrue();

            compiledAssembly.Should().NotBeNull();
            compiledAssembly.DefinedTypes.Should().NotBeNull();
            compiledAssembly.DefinedTypes.Should().HaveCount(1);

            //type check
            Type resultType = compiledAssembly.DefinedTypes.First();
            resultType.Name.Should().Be(testType);
            resultType.Namespace.Should().Be($"{namespacePrefix}.{testNamespace}");
            resultType.IsValueType.Should().BeFalse();
            resultType.CustomAttributes.Should().Contain(attribute => attribute.AttributeType == rosMessageTypeAttributeType);

            MethodInfo equalsMethod = resultType.GetMethod("Equals");
            equalsMethod.Should().NotBeNull();
            equalsMethod.ReturnType.Should().Be(typeof(Boolean));
            equalsMethod.IsPublic.Should().BeTrue();
            equalsMethod.IsVirtual.Should().BeTrue();

            IEnumerable<FieldInfo> resultConstantFieldCollection = resultType.GetFields().Where(field => field.IsLiteral && !field.IsInitOnly).ToList();
            resultConstantFieldCollection.Should().NotBeNull();
            resultConstantFieldCollection.Should().BeEmpty();

            IEnumerable<PropertyInfo> resultPropertyCollection = resultType.GetProperties();
            resultPropertyCollection.Should().NotBeNull();
            resultPropertyCollection.Should().BeEmpty();
        }

        [Test]
        public void RosMessageTemplate_UnitTest_RosMessageTypeAttributeTypeNamespaceIsEmpty_ShouldThrowArgumentException()
        {
            //arrange
            Type rosMessageTypeAttributeType = typeof(RosMessageTypeAttribute);
            string rosbridgeAttributeNamespace = string.Empty;
            string rosbridgeAttributeType = rosMessageTypeAttributeType.Name;
            string namespacePrefix = "testPrefix";
            string testNamespace = "testNamespace";
            string testType = "testType";
            IEnumerable<string> testDependencies = new string[] { };
            IEnumerable<Tuple<string, string, string>> testConstantFields = new List<Tuple<string, string, string>>() { Tuple.Create("String", "TestFieldName1", "TestValue") };
            IEnumerable<Tuple<string, string, int>> testArrayFields = new List<Tuple<string, string, int>>() { Tuple.Create("String", "TestFieldName2", 2) };
            IDictionary<string, string> testFields = new Dictionary<string, string>()
            {
                { "TestFieldName3", "String" }
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
            templateOutput.Should().NotBeNull();
            _textTemplatingEngineHost.Errors.Should().HaveCount(1);
            _textTemplatingEngineHost.Errors.Any(error => error.FileName == _template.FullName && error.ErrorText.Contains(typeof(ArgumentException).Name)).Should().BeTrue();
        }

        [Test]
        public void RosMessageTemplate_UnitTest_RosMessageTypeAttributeTypeNameIsEmpty_ShouldThrowArgumentException()
        {
            //arrange
            Type rosMessageTypeAttributeType = typeof(RosMessageTypeAttribute);
            string rosbridgeAttributeNamespace = rosMessageTypeAttributeType.Namespace;
            string rosbridgeAttributeType = string.Empty;
            string namespacePrefix = "testPrefix";
            string testNamespace = "testNamespace";
            string testType = "testType";
            IEnumerable<string> testDependencies = new string[] { };
            IEnumerable<Tuple<string, string, string>> testConstantFields = new List<Tuple<string, string, string>>() { Tuple.Create("String", "TestFieldName1", "TestValue") };
            IEnumerable<Tuple<string, string, int>> testArrayFields = new List<Tuple<string, string, int>>() { Tuple.Create("String", "TestFieldName2", 2) };
            IDictionary<string, string> testFields = new Dictionary<string, string>()
            {
                { "TestFieldName3", "String" }
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
            templateOutput.Should().NotBeNull();
            _textTemplatingEngineHost.Errors.Should().HaveCount(1);
            _textTemplatingEngineHost.Errors.Any(error => error.FileName == _template.FullName && error.ErrorText.Contains(typeof(ArgumentException).Name)).Should().BeTrue();
        }

        [Test]
        public void RosMessageTemplate_UnitTest_NamespacePrefixIsEmpty_ShouldThrowArgumentException()
        {
            //arrange
            Type rosMessageTypeAttributeType = typeof(RosMessageTypeAttribute);
            string rosbridgeAttributeNamespace = rosMessageTypeAttributeType.Namespace;
            string rosbridgeAttributeType = rosMessageTypeAttributeType.Name;
            string namespacePrefix = string.Empty;
            string testNamespace = "testNamespace";
            string testType = "testType";
            IEnumerable<string> testDependencies = new string[] { };
            IEnumerable<Tuple<string, string, string>> testConstantFields = new List<Tuple<string, string, string>>() { Tuple.Create("String", "TestFieldName1", "TestValue") };
            IEnumerable<Tuple<string, string, int>> testArrayFields = new List<Tuple<string, string, int>>() { Tuple.Create("String", "TestFieldName2", 2) };
            IDictionary<string, string> testFields = new Dictionary<string, string>()
            {
                { "TestFieldName3", "String" }
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
            templateOutput.Should().NotBeNull();
            _textTemplatingEngineHost.Errors.Should().HaveCount(1);
            _textTemplatingEngineHost.Errors.Any(error => error.FileName == _template.FullName && error.ErrorText.Contains(typeof(ArgumentException).Name)).Should().BeTrue();
        }

        [Test]
        public void RosMessageTemplate_UnitTest_MessageNamespaceIsEmpty_ShouldThrowArgumentException()
        {
            //arrange
            Type rosMessageTypeAttributeType = typeof(RosMessageTypeAttribute);
            string rosbridgeAttributeNamespace = rosMessageTypeAttributeType.Namespace;
            string rosbridgeAttributeType = rosMessageTypeAttributeType.Name;
            string namespacePrefix = "testPrefix";
            string testNamespace = string.Empty;
            string testType = "testType";
            IEnumerable<string> testDependencies = new string[] { };
            IEnumerable<Tuple<string, string, string>> testConstantFields = new List<Tuple<string, string, string>>() { Tuple.Create("String", "TestFieldName1", "TestValue") };
            IEnumerable<Tuple<string, string, int>> testArrayFields = new List<Tuple<string, string, int>>() { Tuple.Create("String", "TestFieldName2", 2) };
            IDictionary<string, string> testFields = new Dictionary<string, string>()
            {
                { "TestFieldName3", "String" }
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
            templateOutput.Should().NotBeNull();
            _textTemplatingEngineHost.Errors.Should().HaveCount(1);
            _textTemplatingEngineHost.Errors.Any(error => error.FileName == _template.FullName && error.ErrorText.Contains(typeof(ArgumentException).Name)).Should().BeTrue();
        }

        [Test]
        public void RosMessageTemplate_UnitTest_MessageTypeIsEmpty_ShouldThrowArgumentException()
        {
            //arrange
            Type rosMessageTypeAttributeType = typeof(RosMessageTypeAttribute);
            string rosbridgeAttributeNamespace = rosMessageTypeAttributeType.Namespace;
            string rosbridgeAttributeType = rosMessageTypeAttributeType.Name;
            string namespacePrefix = "testPrefix";
            string testNamespace = "testNamespace";
            string testType = string.Empty;
            IEnumerable<string> testDependencies = new string[] { };
            IEnumerable<Tuple<string, string, string>> testConstantFields = new List<Tuple<string, string, string>>() { Tuple.Create("String", "TestFieldName1", "TestValue") };
            IEnumerable<Tuple<string, string, int>> testArrayFields = new List<Tuple<string, string, int>>() { Tuple.Create("String", "TestFieldName2", 2) };
            IDictionary<string, string> testFields = new Dictionary<string, string>()
            {
                { "TestFieldName3", "String" }
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
            templateOutput.Should().NotBeNull();
            _textTemplatingEngineHost.Errors.Should().HaveCount(1);
            _textTemplatingEngineHost.Errors.Any(error => error.FileName == _template.FullName && error.ErrorText.Contains(typeof(ArgumentException).Name)).Should().BeTrue();
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