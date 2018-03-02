namespace Rosbridge.CodeGeneration.Logic.UnitTests.LogicUnitTests
{
    using BaseClasses;
    using Constants;
    using FluentAssertions;
    using Helpers;
    using Interfaces;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.Collections.Generic;
    using System.IO;

    [TestFixture]
    public class ROSFileLoaderUnitTests
    {
        private Mock<ROSFileLoader> _testClassPartialMock;
        private Mock<IYAMLParser> _yamlParserMock;

        [SetUp]
        public void SetUp()
        {
            _yamlParserMock = new Mock<IYAMLParser>();
            _testClassPartialMock = new Mock<ROSFileLoader>(_yamlParserMock.Object);
        }

        [Test]
        public void LoadRosFiles_UnitTest_ArgumentsOK_ReturnCorrectSetsAllHelperMethodsCalled()
        {
            //arrange
            ISet<MsgFile> messageFileSet = new HashSet<MsgFile>();
            ISet<SrvFile> serviceFileSet = new HashSet<SrvFile>();
            string directoryPath = "testDirectoryPath";

            ISet<FileInfo> messageFileInfoSet = new HashSet<FileInfo>();
            ISet<FileInfo> serviceFileInfoSet = new HashSet<FileInfo>();

            IEnumerable<MsgFile> messageFileCollection = new List<MsgFile>();
            IEnumerable<SrvFile> serviceFileCollection = new List<SrvFile>();

            _testClassPartialMock.Setup(fileLoader => fileLoader.IsDirectoryExists(directoryPath)).Returns(true);
            _testClassPartialMock.Setup(fileLoader => fileLoader.LoadFiles(
                    directoryPath,
                    RosConstants.FileExtensions.MSG_FILE_EXTENSION,
                    SearchOption.AllDirectories))
                .Returns(messageFileInfoSet);
            _testClassPartialMock.Setup(fileLoader => fileLoader.LoadFiles(
                    directoryPath,
                    RosConstants.FileExtensions.MSG_FILE_EXTENSION,
                    SearchOption.AllDirectories))
                .Returns(serviceFileInfoSet);
            _testClassPartialMock.Setup(fileLoader => fileLoader.CreateMessageFileCollection(messageFileInfoSet))
                .Returns(messageFileCollection);
            _testClassPartialMock.Setup(fileLoader => fileLoader.CreateServiceFileCollection(serviceFileInfoSet))
                .Returns(serviceFileCollection);

            //act
            _testClassPartialMock.Object.LoadRosFiles(messageFileSet, serviceFileSet, directoryPath);

            //assert
            messageFileSet.Should().NotBeNull();
            _testClassPartialMock.Verify(fileLoader => fileLoader.IsDirectoryExists(directoryPath), Times.Once);
            _testClassPartialMock.Verify(fileLoader => fileLoader.LoadFiles(directoryPath, RosConstants.FileExtensions.MSG_FILE_EXTENSION, SearchOption.AllDirectories), Times.Once);
            _testClassPartialMock.Verify(fileLoader => fileLoader.LoadFiles(directoryPath, RosConstants.FileExtensions.SRV_FILE_EXTENSION, SearchOption.AllDirectories), Times.Once);
        }

        [Test]
        public void LoadRosFiles_UnitTest_MessageFileSetIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            ISet<MsgFile> messageFileSet = null;
            ISet<SrvFile> serviceFileSet = new HashSet<SrvFile>();
            string directoryPath = "testDirectoryPath";

            //act
            Action act = () => _testClassPartialMock.Object.LoadRosFiles(messageFileSet, serviceFileSet, directoryPath);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void LoadRosFiles_UnitTest_ServiceFileSetIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            ISet<MsgFile> messageFileSet = new HashSet<MsgFile>();
            ISet<SrvFile> serviceFileSet = null;
            string directoryPath = "testDirectoryPath";

            //act
            Action act = () => _testClassPartialMock.Object.LoadRosFiles(messageFileSet, serviceFileSet, directoryPath);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void LoadRosFiles_UnitTest_DirectoryPathIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            ISet<MsgFile> messageFileSet = new HashSet<MsgFile>();
            ISet<SrvFile> serviceFileSet = new HashSet<SrvFile>();
            string directoryPath = null;

            //act
            Action act = () => _testClassPartialMock.Object.LoadRosFiles(messageFileSet, serviceFileSet, directoryPath);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void LoadRosFiles_UnitTest_DirectoryPathIsEmpty_ShouldThrowArgumentNullException()
        {
            //arrange
            ISet<MsgFile> messageFileSet = new HashSet<MsgFile>();
            ISet<SrvFile> serviceFileSet = new HashSet<SrvFile>();
            string directoryPath = string.Empty;

            //act
            Action act = () => _testClassPartialMock.Object.LoadRosFiles(messageFileSet, serviceFileSet, directoryPath);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void LoadRosFiles_UnitTest_IsDirectoryExistsMethodReturnFalse_ShouldThrowDirectoryNotFoundException()
        {
            //arrange
            ISet<MsgFile> messageFileSet = new HashSet<MsgFile>();
            ISet<SrvFile> serviceFileSet = new HashSet<SrvFile>();
            string directoryPath = "testDirectoryPath";

            _testClassPartialMock.Setup(fileLoader => fileLoader.IsDirectoryExists(directoryPath)).Returns(false);

            //act
            Action act = () => _testClassPartialMock.Object.LoadRosFiles(messageFileSet, serviceFileSet, directoryPath);

            //assert
            act.Should().ThrowExactly<DirectoryNotFoundException>();
        }
    }
}