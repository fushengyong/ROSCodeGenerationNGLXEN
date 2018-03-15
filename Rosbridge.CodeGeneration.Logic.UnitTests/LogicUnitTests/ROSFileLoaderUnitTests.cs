namespace Rosbridge.CodeGeneration.Logic.UnitTests.LogicUnitTests
{
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
        public void LoadRosFiles_ReturnCorrectSetsAllHelperMethodsCalled()
        {
            //arrange
            ISet<IMsgFile> messageFileSet = new HashSet<IMsgFile>();
            ISet<ISrvFile> serviceFileSet = new HashSet<ISrvFile>();
            string directoryPath = "testDirectoryPath";

            ISet<FileInfo> messageFileInfoSet = new HashSet<FileInfo>();
            ISet<FileInfo> serviceFileInfoSet = new HashSet<FileInfo>();

            IEnumerable<IMsgFile> messageFileCollection = new List<IMsgFile>();
            IEnumerable<ISrvFile> serviceFileCollection = new List<ISrvFile>();

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
        public void LoadRosFiles_MessageFileSetIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            ISet<IMsgFile> messageFileSet = null;
            ISet<ISrvFile> serviceFileSet = new HashSet<ISrvFile>();
            string directoryPath = "testDirectoryPath";

            //act
            Action act = () => _testClassPartialMock.Object.LoadRosFiles(messageFileSet, serviceFileSet, directoryPath);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void LoadRosFiles_ServiceFileSetIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            ISet<IMsgFile> messageFileSet = new HashSet<IMsgFile>();
            ISet<ISrvFile> serviceFileSet = null;
            string directoryPath = "testDirectoryPath";

            //act
            Action act = () => _testClassPartialMock.Object.LoadRosFiles(messageFileSet, serviceFileSet, directoryPath);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void LoadRosFiles_DirectoryPathIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            ISet<IMsgFile> messageFileSet = new HashSet<IMsgFile>();
            ISet<ISrvFile> serviceFileSet = new HashSet<ISrvFile>();
            string directoryPath = null;

            //act
            Action act = () => _testClassPartialMock.Object.LoadRosFiles(messageFileSet, serviceFileSet, directoryPath);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void LoadRosFiles_DirectoryPathIsEmpty_ShouldThrowArgumentNullException()
        {
            //arrange
            ISet<IMsgFile> messageFileSet = new HashSet<IMsgFile>();
            ISet<ISrvFile> serviceFileSet = new HashSet<ISrvFile>();
            string directoryPath = string.Empty;

            //act
            Action act = () => _testClassPartialMock.Object.LoadRosFiles(messageFileSet, serviceFileSet, directoryPath);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void LoadRosFiles_IsDirectoryExistsMethodReturnFalse_ShouldThrowDirectoryNotFoundException()
        {
            //arrange
            ISet<IMsgFile> messageFileSet = new HashSet<IMsgFile>();
            ISet<ISrvFile> serviceFileSet = new HashSet<ISrvFile>();
            string directoryPath = "testDirectoryPath";

            _testClassPartialMock.Setup(fileLoader => fileLoader.IsDirectoryExists(directoryPath)).Returns(false);

            //act
            Action act = () => _testClassPartialMock.Object.LoadRosFiles(messageFileSet, serviceFileSet, directoryPath);

            //assert
            act.Should().ThrowExactly<DirectoryNotFoundException>();
        }
    }
}