namespace Rosbridge.CodeGeneration.Logic.UnitTests.LogicUnitTests
{
    using BaseClasses;
    using EnvDTE;
    using Exceptions;
    using FluentAssertions;
    using Microsoft.VisualStudio.TextTemplating;
    using Microsoft.VisualStudio.TextTemplating.VSHost;
    using Moq;
    using NUnit.Framework;
    using Rosbridge.CodeGeneration.Logic.Constants;
    using Rosbridge.CodeGeneration.Logic.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    [TestFixture]
    public class CodeGeneratorUnitTests
    {
        private const string ROS_MESSAGE_PROJECT_NAME = "testProjectName";
        private const string ROS_MESSAGE_TYPE_ATTRIBUTE_NAME = "testAttributeName";
        private const string ROS_MESSAGE_TYPE_ATTRIBUTE_NAMESPACE = "testAttributeNamespace";
        private const string ROS_MESSAGE_CODE_GENERATION_TEMPLATE_PATH = "testTemplatePath";
        private const string ROS_MESSAGE_CODE_GENERATION_TEMPLATE_CONTENT = "templateContent";
        private const string CUSTOM_TIME_DATA_TEMPLATE_PATH = "testCustomTimeDataTemplatePath";

        private Mock<Helpers.CodeGenerator> _testClassPartialMock;
        private Mock<ITextTemplatingEngineHost> _textTemplatingEngineHostMock;
        private Mock<ITextTemplating> _textTemplatingMock;
        private Mock<ITextTemplatingSessionHost> _textTemplatingSessionHostMock;
        private Mock<ISolutionManager> _solutionManagerMock;

        [SetUp]
        public void SetUp()
        {
            _textTemplatingEngineHostMock = new Mock<ITextTemplatingEngineHost>();
            _textTemplatingMock = new Mock<ITextTemplating>();
            _textTemplatingSessionHostMock = new Mock<ITextTemplatingSessionHost>();
            _solutionManagerMock = new Mock<ISolutionManager>();

            _testClassPartialMock = new Mock<Helpers.CodeGenerator>(_textTemplatingEngineHostMock.Object,
                _textTemplatingSessionHostMock.Object, _textTemplatingMock.Object, _solutionManagerMock.Object,
                ROS_MESSAGE_PROJECT_NAME, ROS_MESSAGE_TYPE_ATTRIBUTE_NAME, ROS_MESSAGE_TYPE_ATTRIBUTE_NAMESPACE);

            _textTemplatingEngineHostMock
                .Setup(textTemplatingEngineHost =>
                    textTemplatingEngineHost.ResolvePath(Helpers.CodeGenerator.ROS_MESSAGE_CODE_GENERATION_TEMPLATE_RELATIVE_PATH))
                .Returns(ROS_MESSAGE_CODE_GENERATION_TEMPLATE_PATH);
            _testClassPartialMock
                .Setup(testClass => testClass.ReadAllTextFromFile(ROS_MESSAGE_CODE_GENERATION_TEMPLATE_PATH))
                .Returns(ROS_MESSAGE_CODE_GENERATION_TEMPLATE_CONTENT);
            _textTemplatingEngineHostMock
                .Setup(textTemplatingEngineHost => textTemplatingEngineHost.ResolvePath(Helpers.CodeGenerator.CUSTOM_TIME_DATA_TEMPLATE_RELATIVE_PATH))
                .Returns(CUSTOM_TIME_DATA_TEMPLATE_PATH);
        }

        [Test]
        public void GenerateRosMessages_UnitTest_ArgumentsOK_ShouldCallGenerateStandardNamespaceMessagesAndGenerateMessagesByNamespaceMethods()
        {
            //arrange
            string testType = "testType";
            string nonStandardNamespace = "testNamespace";
            string standardNamespace = "testStandardNamespace";
            Mock<IMsgFile> standardNamespaceMsgFileMock = new Mock<IMsgFile>();
            Mock<IMsgFile> messageFileMock = new Mock<IMsgFile>();
            ISet<IMsgFile> messageFileSet = new HashSet<IMsgFile>();
            messageFileSet.Add(standardNamespaceMsgFileMock.Object);
            messageFileSet.Add(messageFileMock.Object);

            standardNamespaceMsgFileMock.SetupGet(standardNamespaceMsgFile => standardNamespaceMsgFile.Type)
                .Returns(new RosType(standardNamespace, testType));
            messageFileMock.SetupGet(messageFile => messageFile.Type)
                .Returns(new RosType(nonStandardNamespace, testType));

            //act
            _testClassPartialMock.Object.GenerateRosMessages(messageFileSet, standardNamespace);

            //assert
            _testClassPartialMock.Verify(testClass =>
                testClass.GenerateStandardNamespaceMessages(It.Is<IEnumerable<IGrouping<string, IMsgFile>>>(groupList => groupList.Any(group => group.Key == standardNamespace)), standardNamespace), Times.Once);
            _testClassPartialMock.Verify(testClass =>
                testClass.GenerateMessagesByNamespace(It.Is<IGrouping<string, IMsgFile>>(group => group.Key == nonStandardNamespace), standardNamespace), Times.Once);
        }

        [Test]
        public void GenerateRosMessages_UnitTest_MessageSetIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            string standardNamespace = "testStandardNamespace";
            ISet<IMsgFile> messageFileSet = null;

            //act
            Action act = () => _testClassPartialMock.Object.GenerateRosMessages(messageFileSet, standardNamespace);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void GenerateRosMessages_UnitTest_StandardNamespaceIsNull_ShouldThrowArgumentException()
        {
            //arrange
            string standardNamespace = null;
            ISet<IMsgFile> messageFileSet = new HashSet<IMsgFile>();

            //act
            Action act = () => _testClassPartialMock.Object.GenerateRosMessages(messageFileSet, standardNamespace);

            //assert
            act.Should().ThrowExactly<NoStandardNamespaceException>();
        }

        [Test]
        public void GenerateRosMessages_UnitTest_StandardNamespaceIsEmpty_ShouldThrowArgumentException()
        {
            //arrange
            string standardNamespace = string.Empty;
            ISet<IMsgFile> messageFileSet = new HashSet<IMsgFile>();

            //act
            Action act = () => _testClassPartialMock.Object.GenerateRosMessages(messageFileSet, standardNamespace);

            //assert
            act.Should().ThrowExactly<NoStandardNamespaceException>();
        }

        [Test]
        public void GenerateStandardNamespaceMessages_UnitTest_ArgumentsOK_ShouldCallGenerateMessagesByNamespaceAndGenerateCustomTimePrimitiveTypeMethod()
        {
            //arrange
            string testType = "testType";
            string standardNamespace = "testStandardNamespace";
            Mock<IMsgFile> msgFileMock = new Mock<IMsgFile>();
            IEnumerable<IGrouping<string, IMsgFile>> groupList = new IMsgFile[] { msgFileMock.Object }.GroupBy(msgFile => msgFile.Type.Namespace);
            Mock<ProjectItem> standardNamespaceDirectoryProjectItemMock = new Mock<ProjectItem>();

            _testClassPartialMock.CallBase = true;
            msgFileMock.Setup(msgFile => msgFile.Type).Returns(new RosType(standardNamespace, testType));
            _testClassPartialMock
                .Setup(testClass =>
                    testClass.GenerateMessagesByNamespace(It.IsAny<IGrouping<string, IMsgFile>>(), It.IsAny<string>()))
                .Returns(standardNamespaceDirectoryProjectItemMock.Object);
            _testClassPartialMock.Setup(testClass =>
                testClass.GenerateCustomTimePrimitiveType(standardNamespaceDirectoryProjectItemMock.Object,
                    standardNamespace));

            //act
            _testClassPartialMock.Object.GenerateStandardNamespaceMessages(groupList, standardNamespace);

            //assert
            _testClassPartialMock.Verify(testClass => testClass.GenerateMessagesByNamespace(It.Is<IGrouping<string, IMsgFile>>(group => group.Key == standardNamespace), standardNamespace), Times.Once);
            _testClassPartialMock.Verify(testClass => testClass.GenerateCustomTimePrimitiveType(standardNamespaceDirectoryProjectItemMock.Object, standardNamespace), Times.Once);
        }

        [Test]
        public void GenerateStandardNamespaceMessages_UnitTest_NoStandardNamespaceGroup_ShouldThrowNoStandardNamespaceException()
        {
            //arrange
            string testType = "testType";
            string standardNamespace = "testStandardNamespace";
            string nonStandardNamepsace = "testNonStandardNamespace";
            Mock<IMsgFile> msgFileMock = new Mock<IMsgFile>();
            IEnumerable<IGrouping<string, IMsgFile>> groupList = new IMsgFile[] { msgFileMock.Object }.GroupBy(msgFile => msgFile.Type.Namespace);

            _testClassPartialMock.CallBase = true;
            msgFileMock.Setup(msgFile => msgFile.Type).Returns(new RosType(nonStandardNamepsace, testType));

            //act
            Action act = () => _testClassPartialMock.Object.GenerateStandardNamespaceMessages(groupList, standardNamespace);

            //assert
            act.Should().ThrowExactly<NoStandardNamespaceException>();
        }

        [Test]
        public void GenerateCustomTimePrimitiveType_UnitTest_ArgumentsOK_ShouldCallTransformTemplateToFileWithAppropriateArguments()
        {
            //arrange
            Mock<ProjectItem> standardNamespaceDirectoryProjectItemMock = new Mock<ProjectItem>();
            string standardNamespace = "testStandardNamespace";
            Mock<ITextTemplatingSession> textTemplatingSessionMock = new Mock<ITextTemplatingSession>();
            string templateNamespace = $"{ROS_MESSAGE_PROJECT_NAME}.{standardNamespace}";
            string templateContent = "testTemplateContent";

            _testClassPartialMock.CallBase = true;
            _textTemplatingSessionHostMock.Setup(textTemplatingSession => textTemplatingSession.CreateSession())
                .Returns(textTemplatingSessionMock.Object);
            _testClassPartialMock.Setup(testClass =>
                testClass.TransformTemplateToFile(It.IsAny<ITextTemplatingSession>(), It.IsAny<ProjectItem>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Callback(
                () => { });
            _testClassPartialMock
                .Setup(testClass => testClass.ReadAllTextFromFile(CUSTOM_TIME_DATA_TEMPLATE_PATH))
                .Returns(templateContent);

            //act
            _testClassPartialMock.Object.GenerateCustomTimePrimitiveType(standardNamespaceDirectoryProjectItemMock.Object, standardNamespace);

            //assert
            _testClassPartialMock.Verify(testClass =>
                testClass.TransformTemplateToFile(
                    textTemplatingSessionMock.Object,
                    standardNamespaceDirectoryProjectItemMock.Object,
                    CUSTOM_TIME_DATA_TEMPLATE_PATH, templateContent,
                    RosConstants.MessageTypes.CUSTOM_TIME_PRIMITIVE_TYPE), Times.Once);
        }

        [Test]
        public void GenerateMessagesByNamespace_UnitTest_ArgumentsOK_ShouldCallAddNewDirectoryToProjectAndGenerateMessagesMethods()
        {
            //arrange
            string testType = "testType";
            string testNamespace = "testNamespace";
            string standardNamespace = "testStandardNamespace";
            Mock<IMsgFile> messageFileMock = new Mock<IMsgFile>();
            messageFileMock.SetupGet(msgFile => msgFile.Type).Returns(new RosType(testNamespace, testType));
            Mock<ProjectItem> groupDirectoryProjectItemMock = new Mock<ProjectItem>();
            IGrouping<string, IMsgFile> msgGroup = new IMsgFile[] { messageFileMock.Object }.GroupBy(msg => msg.Type.Namespace).First();

            _testClassPartialMock.CallBase = true;
            _solutionManagerMock.Setup(solutionManager => solutionManager.AddNewDirectoryToProject(msgGroup.Key))
                .Returns(groupDirectoryProjectItemMock.Object);
            _testClassPartialMock.Setup(testClass =>
                testClass.GenerateMessages(groupDirectoryProjectItemMock.Object, msgGroup, standardNamespace)).Callback(() => { });

            //act
            ProjectItem result = _testClassPartialMock.Object.GenerateMessagesByNamespace(msgGroup, standardNamespace);

            //assert
            result.Should().NotBeNull();
            result.Should().BeSameAs(groupDirectoryProjectItemMock.Object);

            _testClassPartialMock.Verify(testClass => testClass.GenerateMessages(groupDirectoryProjectItemMock.Object, msgGroup, standardNamespace));
        }

        [Test]
        public void GenerateMessages_UnitTest_ArgumentsOK_ShouldCallTransformTemplateToFileMethod()
        {
            //arrange
            string messageType = "testType";
            string standardNamespace = "testStandardNamespace";
            string messageNamespace = "testNamespace";
            Mock<ProjectItem> directoryProjectItemMock = new Mock<ProjectItem>();
            Mock<IMsgFile> messageFileMock = new Mock<IMsgFile>();
            IEnumerable<IMsgFile> messageFileCollection = new IMsgFile[] { messageFileMock.Object };
            Mock<ITextTemplatingSession> templatingSessionMock = new Mock<ITextTemplatingSession>();

            RosType testDependencyType = new RosType("testDepdendencyNamespace", "testDependencyType");
            ISet<RosType> dependencySet = new HashSet<RosType>();
            dependencySet.Add(testDependencyType);

            ISet<MessageField> arrayFieldSet = new HashSet<MessageField>();
            ISet<MessageField> constantFieldSet = new HashSet<MessageField>();
            ISet<MessageField> fieldSet = new HashSet<MessageField>();
            messageFileMock.SetupGet(messageFile => messageFile.ArrayFieldSet).Returns(arrayFieldSet);
            messageFileMock.SetupGet(messageFile => messageFile.ConstantFieldSet).Returns(constantFieldSet);
            messageFileMock.SetupGet(messageFile => messageFile.FieldSet).Returns(fieldSet);

            _testClassPartialMock.CallBase = true;

            messageFileMock.SetupGet(messageFile => messageFile.Type).Returns(new RosType(messageNamespace, messageType));
            messageFileMock.SetupGet(messageFile => messageFile.DependencySet).Returns(dependencySet);
            _textTemplatingSessionHostMock.Setup(textTemplatingSessionHost => textTemplatingSessionHost.CreateSession())
                .Returns(templatingSessionMock.Object);
            _textTemplatingSessionHostMock.Setup(textTemplatingSessionHost => textTemplatingSessionHost.CreateSession())
                .Returns(templatingSessionMock.Object);
            _testClassPartialMock.Setup(testClass => testClass.TransformTemplateToFile(templatingSessionMock.Object,
                directoryProjectItemMock.Object, ROS_MESSAGE_CODE_GENERATION_TEMPLATE_PATH,
                ROS_MESSAGE_CODE_GENERATION_TEMPLATE_CONTENT, It.IsAny<string>())).Callback(() => { });

            //act
            _testClassPartialMock.Object.GenerateMessages(directoryProjectItemMock.Object, messageFileCollection, standardNamespace);

            //assert
            _textTemplatingSessionHostMock.Verify(sessionHost => sessionHost.CreateSession(), Times.Once);
            _testClassPartialMock.Verify(testClass =>
                testClass.TransformTemplateToFile(templatingSessionMock.Object, directoryProjectItemMock.Object, ROS_MESSAGE_CODE_GENERATION_TEMPLATE_PATH, ROS_MESSAGE_CODE_GENERATION_TEMPLATE_CONTENT, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void TransformTemplateToFile_UnitTest_ArgumentsOK_ShouldCallMethodsWithAppropriateArguments()
        {
            //arrange
            Mock<ITextTemplatingSession> textTemplatingSessionMock = new Mock<ITextTemplatingSession>();
            Mock<ProjectItem> groupDirectoryProjectItemMock = new Mock<ProjectItem>();
            string templatePath = "templatePath";
            string templateContent = "templateContent";
            string typeName = "typeName";
            string directoryPath = "directoryPath";
            string transformedTemplate = "testTransformedTemplate";
            string newFilePath = "testNewFilePath";

            _testClassPartialMock.CallBase = true;

            _solutionManagerMock
                .Setup(solutionManager => solutionManager.GetProjectItemFullPath(groupDirectoryProjectItemMock.Object))
                .Returns(directoryPath);
            _textTemplatingMock.Setup(textTemplating => textTemplating.ProcessTemplate(templatePath, templateContent, null, null))
                .Returns(transformedTemplate);
            _testClassPartialMock
                .Setup(testClass => testClass.WriteToFile(directoryPath, typeName, transformedTemplate)).Returns(newFilePath);

            //act
            _testClassPartialMock.Object.TransformTemplateToFile(textTemplatingSessionMock.Object, groupDirectoryProjectItemMock.Object, templatePath, templateContent, typeName);

            //assert
            _solutionManagerMock.Verify(solutionManager => solutionManager.GetProjectItemFullPath(groupDirectoryProjectItemMock.Object), Times.Once);
            _textTemplatingMock.Verify(textTemplating => textTemplating.ProcessTemplate(templatePath, templateContent, null, null), Times.Once);
            _testClassPartialMock.Verify(testClass => testClass.WriteToFile(directoryPath, typeName, transformedTemplate), Times.Once);
            _solutionManagerMock.Verify(solutionManager => solutionManager.AddFileToDirectoryProjectItem(groupDirectoryProjectItemMock.Object, newFilePath), Times.Once);
        }

        [Test]
        public void WriteToFile_UnitTest_ArgumentsOk_ShouldReturnCorrectPathShouldCallWriteAllTextToFileMethod()
        {
            //arrange
            string directoryPath = "testDirectoryPath";
            string typeName = "testTypeName";
            string fileContent = "testFileContent";
            string newFilePath = Path.Combine(directoryPath, $"{typeName}.cs");

            _testClassPartialMock.CallBase = true;

            _testClassPartialMock.Setup(testCLass => testCLass.WriteAllTextToFile(newFilePath, fileContent))
                .Callback(() => { });

            //act
            string result = _testClassPartialMock.Object.WriteToFile(directoryPath, typeName, fileContent);

            //assert
            result.Should().NotBeNull();
            result.Should().Be(newFilePath);
            _testClassPartialMock.Verify(testClass => testClass.WriteAllTextToFile(newFilePath, fileContent), Times.Once);
        }
    }
}
