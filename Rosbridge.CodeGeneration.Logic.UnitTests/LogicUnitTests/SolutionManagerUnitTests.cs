namespace Rosbridge.CodeGeneration.Logic.UnitTests.LogicUnitTests
{
    using EnvDTE;
    using EnvDTE80;
    using Exceptions;
    using FluentAssertions;
    using Helpers;
    using Moq;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using System;
    using System.IO;
    using VSLangProj;

    [TestFixture]
    public class SolutionManagerUnitTests
    {
        private Mock<SolutionManager> _testClassPartialMock;
        private Mock<IServiceProvider> _serviceProviderMock;
        private Mock<DTE> _dteMock;
        private Mock<Solution2> _solutionMock;
        private string _projectName;
        private string _rosAttributeProjectName;
        private string _projectTemplate;

        [SetUp]
        public void SetUp()
        {
            _serviceProviderMock = new Mock<IServiceProvider>();
            _dteMock = new Mock<DTE>();
            _solutionMock = new Mock<Solution2>();
            _projectName = "testProject";
            _rosAttributeProjectName = "testRosAttributeProjectName";
            _projectTemplate = "testProjectTemplate";

            _serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()))
                .Returns(_dteMock.Object);
            _solutionMock.As<Solution>();
            _dteMock.SetupGet(dte => dte.Solution).Returns((Solution)_solutionMock.Object);

            _testClassPartialMock = new Mock<SolutionManager>(_serviceProviderMock.Object, _projectName, _rosAttributeProjectName, _projectTemplate);
        }

        [Test]
        public void Initialize_UnitTest_EverythingOK_ProjectFieldShouldSetMethodsShouldBeCalled()
        {
            //arrange
            Mock<Project> currentProjectMock = new Mock<Project>();
            Mock<Project> clientProjectMock = new Mock<Project>();
            Mock<VSProject> vsProjectMock = new Mock<VSProject>();
            Mock<References> referencesMock = new Mock<References>();
            string solutionPathMock = "testSolution";
            string projectPathMock = Path.Combine(solutionPathMock, _projectName);
            string classLibraryTemplatePath = "testClassLibraryTemplatePath";

            _testClassPartialMock.Setup(testClass => testClass.GetProjectByName(_projectName))
                .Returns(currentProjectMock.Object);
            _solutionMock.Setup(solution => solution.Remove(currentProjectMock.Object));
            _solutionMock.SetupGet(solution => solution.FullName).Returns(solutionPathMock);
            _solutionMock.Setup(solution => solution.GetProjectTemplate(classLibraryTemplatePath, It.IsAny<string>())).Returns(classLibraryTemplatePath);
            _solutionMock.Setup(solution =>
                solution.AddFromTemplate(classLibraryTemplatePath, projectPathMock, _projectName, false));
            _testClassPartialMock.Setup(testClass => testClass.GetProjectByName(_rosAttributeProjectName))
                .Returns(clientProjectMock.Object);
            currentProjectMock.SetupGet(newProject => newProject.Object).Returns((object)vsProjectMock.Object);
            vsProjectMock.SetupGet(vsProject => vsProject.References).Returns(referencesMock.Object);
            referencesMock.Setup(references => references.AddProject(clientProjectMock.Object));

            //act
            _testClassPartialMock.Object.Initialize();

            //assert
            _testClassPartialMock.Object._project.Should().NotBeNull();
            _testClassPartialMock.Object._project.Should().BeSameAs(currentProjectMock.Object);

            _testClassPartialMock.Verify(testClass => testClass.GetProjectByName(_projectName), Times.Exactly(2));
            _solutionMock.Verify(solution => solution.Remove(currentProjectMock.Object), Times.Once);
            //_testClassPartialMock.Verify(testClass => testClass.TryDeleteDirectory(projectPathMock, true), Times.Once);
            _solutionMock.Verify(solution => solution.GetProjectTemplate(_projectTemplate, It.IsAny<string>()), Times.Once);
            //_solutionMock.Verify(solution => solution.AddFromTemplate(classLibraryTemplatePath, projectPathMock, _projectName, false), Times.Once);
            _testClassPartialMock.Verify(testClass => testClass.DeleteDefaultClass(currentProjectMock.Object), Times.Once);
            _testClassPartialMock.Verify(testClass => testClass.GetProjectByName(_rosAttributeProjectName), Times.Once);
            currentProjectMock.Verify(newProject => newProject.Object, Times.Once);
            vsProjectMock.Verify(vsProject => vsProject.References, Times.Once);
            referencesMock.Verify(references => references.AddProject(clientProjectMock.Object), Times.Once);
        }

        [Test]
        public void Initialize_UnitTest_ClientProjectIsNull_ShouldThrowProjectNotFoundException()
        {
            //arrange
            Mock<Project> currentProjectMock = new Mock<Project>();
            Mock<Project> clientProjectMock = new Mock<Project>();
            string solutionPathMock = "testSolution";
            string projectPathMock = Path.Combine(Path.GetDirectoryName(solutionPathMock), _projectName);
            string classLibraryTemplatePath = "testClassLibraryTemplatePath";

            _testClassPartialMock.Setup(testClass => testClass.GetProjectByName(_projectName))
                .Returns(currentProjectMock.Object);
            _solutionMock.Setup(solution => solution.Remove(currentProjectMock.Object));
            _solutionMock.SetupGet(solution => solution.FullName).Returns(solutionPathMock);
            _solutionMock.Setup(solution => solution.GetProjectTemplate(classLibraryTemplatePath, It.IsAny<string>())).Returns(classLibraryTemplatePath);
            _solutionMock.Setup(solution =>
                solution.AddFromTemplate(classLibraryTemplatePath, projectPathMock, _projectName, false));

            //act
            Action act = () => _testClassPartialMock.Object.Initialize();

            //assert
            act.Should().ThrowExactly<ProjectNotFoundException>();

            _testClassPartialMock.Verify(testClass => testClass.GetProjectByName(_projectName), Times.Exactly(2));
            _solutionMock.Verify(solution => solution.Remove(currentProjectMock.Object), Times.Once);
            _testClassPartialMock.Verify(testClass => testClass.TryDeleteDirectory(projectPathMock, true), Times.Once);
            _solutionMock.Verify(solution => solution.GetProjectTemplate(_projectTemplate, It.IsAny<string>()), Times.Once);
            //_solutionMock.Verify(solution => solution.AddFromTemplate(classLibraryTemplatePath, projectPathMock, _projectName, false), Times.Once);
            _testClassPartialMock.Verify(testClass => testClass.DeleteDefaultClass(currentProjectMock.Object), Times.Once);
            _testClassPartialMock.Verify(testClass => testClass.GetProjectByName(_rosAttributeProjectName), Times.Once);
        }

        [Test]
        public void AddFileToDirectoryProjectItem_UnitTest_ArgumentsOK_ShouldCallProjectItemsAddFromFileMethod()
        {
            //arrange
            Mock<ProjectItem> projectItemMock = new Mock<ProjectItem>();
            Mock<ProjectItems> projectItemsMock = new Mock<ProjectItems>();
            string filePath = "testFilePath";

            projectItemMock.SetupGet(projectItem => projectItem.Kind).Returns(SolutionManager.PROJECT_DIRECTORY_GUID);
            projectItemMock.Setup(projectItem => projectItem.ProjectItems).Returns(projectItemsMock.Object);
            _testClassPartialMock.Setup(testClass => testClass.IsFileExists(filePath)).Returns(true);
            projectItemsMock.Setup(projectItems => projectItems.AddFromFile(filePath));

            //act
            _testClassPartialMock.Object.AddFileToDirectoryProjectItem(projectItemMock.Object, filePath);

            //assert
            _testClassPartialMock.Verify(testClass => testClass.IsFileExists(filePath), Times.Once);
            projectItemMock.Verify(projectItem => projectItem.ProjectItems, Times.Once);
            projectItemsMock.Verify(projectItems => projectItems.AddFromFile(filePath), Times.Once);
        }

        [Test]
        public void AddFileToDirectoryProjectItem_UnitTest_ProjectItemIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            string filePath = "testPath";

            //act
            Action act = () => _testClassPartialMock.Object.AddFileToDirectoryProjectItem(null, filePath);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void AddFileToDirectoryProjectItem_UnitTest_FilePathIsNull_ShouldThrowArgumentException()
        {
            //arrange
            Mock<ProjectItem> projectItemMock = new Mock<ProjectItem>();
            string filePath = null;

            projectItemMock.SetupGet(projectItem => projectItem.Kind).Returns(SolutionManager.PROJECT_DIRECTORY_GUID);

            //act
            Action act = () => _testClassPartialMock.Object.AddFileToDirectoryProjectItem(projectItemMock.Object, filePath);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void AddFileToDirectoryProjectItem_UnitTest_FilePathIsEmpty_ShouldThrowArgumentException()
        {
            //arrange
            Mock<ProjectItem> projectItemMock = new Mock<ProjectItem>();
            string filePath = string.Empty;

            projectItemMock.SetupGet(projectItem => projectItem.Kind).Returns(SolutionManager.PROJECT_DIRECTORY_GUID);

            //act
            Action act = () => _testClassPartialMock.Object.AddFileToDirectoryProjectItem(projectItemMock.Object, filePath);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void AddFileToDirectoryProjectItem_UnitTest_ProjectItemKindIsNotValid_ShouldThrowInvalidOperationException()
        {
            //arrange
            Mock<ProjectItem> projectItemMock = new Mock<ProjectItem>();
            string filePath = "testPath";

            projectItemMock.SetupGet(projectItem => projectItem.Kind).Returns(string.Empty);
            _testClassPartialMock.Setup(testClass => testClass.IsFileExists(filePath)).Returns(true);

            //act
            Action act = () => _testClassPartialMock.Object.AddFileToDirectoryProjectItem(projectItemMock.Object, filePath);

            //assert
            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Test]
        public void AddFileToDirectoryProjectItem_UnitTest_FileNotExists_ShouldThrowFileNotFoundException()
        {
            //arrange
            Mock<ProjectItem> projectItemMock = new Mock<ProjectItem>();
            string filePath = "testPath";

            projectItemMock.SetupGet(projectItem => projectItem.Kind).Returns(SolutionManager.PROJECT_DIRECTORY_GUID);
            _testClassPartialMock.Setup(testClass => testClass.IsFileExists(filePath)).Returns(false);

            //act
            Action act = () => _testClassPartialMock.Object.AddFileToDirectoryProjectItem(projectItemMock.Object, filePath);

            //assert
            act.Should().ThrowExactly<FileNotFoundException>();
        }

        [Test]
        public void AddNewDirectoryToProject_UnitTest_ArgumentOK_TryDeleteDirectoryAndProjectItemsAddFolderMethodShouldCalled()
        {
            //arrange
            Mock<Project> projectMock = new Mock<Project>();
            Mock<ProjectItems> projectItemsMock = new Mock<ProjectItems>();
            string projectPath = "testProjectPath";
            string newDirectoryName = "testDirectory";
            string directoryPathMock = Path.Combine(Path.GetDirectoryName(projectPath), newDirectoryName);

            _testClassPartialMock.Object._project = projectMock.Object;
            projectMock.SetupGet(project => project.FullName).Returns(projectPath);
            projectMock.SetupGet(project => project.ProjectItems).Returns(projectItemsMock.Object);

            //act
            _testClassPartialMock.Object.AddNewDirectoryToProject(newDirectoryName);

            //assert
            _testClassPartialMock.Verify(testClass => testClass.TryDeleteDirectory(directoryPathMock, true), Times.Once);
            projectMock.Verify(project => project.ProjectItems, Times.Once);
            projectItemsMock.Verify(projectItems => projectItems.AddFolder(newDirectoryName, SolutionManager.PROJECT_DIRECTORY_GUID), Times.Once);
        }

        [Test]
        public void AddNewDirectoryToProject_UnitTest_ProjectIsNull_ShouldThrowInvalidOperationException()
        {
            //arrange
            string newDirectoryName = "testDirectory";

            //act
            Action act = () => _testClassPartialMock.Object.AddNewDirectoryToProject(newDirectoryName);

            //assert
            act.Should().ThrowExactly<InvalidOperationException>();
        }

        [Test]
        public void AddNewDirectoryToProject_UnitTest_NewDirectoryNameIsNull_ShouldThrowArgumentException()
        {
            //arrange
            Mock<Project> projectMock = new Mock<Project>();
            string newDirectoryName = null;

            _testClassPartialMock.Object._project = projectMock.Object;

            //act
            Action act = () => _testClassPartialMock.Object.AddNewDirectoryToProject(newDirectoryName);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void AddNewDirectoryToProject_UnitTest_NewDirectoryNameIsEmpty_ShouldThrowArgumentException()
        {
            //arrange
            Mock<Project> projectMock = new Mock<Project>();
            string newDirectoryName = string.Empty;

            _testClassPartialMock.Object._project = projectMock.Object;

            //act
            Action act = () => _testClassPartialMock.Object.AddNewDirectoryToProject(newDirectoryName);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void GetProjectItemFullPath_UnitTest_ProjectItemOK_ShouldCallProjectItemsAppropriateProperty()
        {
            //arrange
            Mock<ProjectItem> projectItemMock = new Mock<ProjectItem>();
            Mock<Properties> propertiesMock = new Mock<Properties>();
            Mock<Property> propertyMock = new Mock<Property>();
            string propertyValue = "testPropertyValue";

            projectItemMock.SetupGet(projectItem => projectItem.Properties).Returns(propertiesMock.Object);
            propertiesMock.Setup(properties => properties.Item(SolutionManager.FULL_PATH_ITEM_PROPERTY))
                .Returns(propertyMock.Object);
            propertyMock.SetupGet(property => property.Value).Returns(propertyValue);

            //act
            string result = _testClassPartialMock.Object.GetProjectItemFullPath(projectItemMock.Object);

            //assert
            result.Should().Be(propertyValue);

            projectItemMock.Verify(projectItem => projectItem.Properties, Times.Once);
            propertiesMock.Verify(properties => properties.Item(SolutionManager.FULL_PATH_ITEM_PROPERTY), Times.Once);
            propertyMock.Verify(property => property.Value, Times.Once);
        }

        [Test]
        public void GetProjectItemFullPath_UnitTest_ProjectItemIsNull_ShouldThrowArgumentNullException()
        {
            //arrange

            //act
            Action act = () => _testClassPartialMock.Object.GetProjectItemFullPath(null);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void Constructor_UnitTest_ArgumentsOK_FieldsShouldSetCorrectly()
        {
            //arrange
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            Mock<DTE> dteMock = new Mock<DTE>();
            Mock<Solution2> solutionMock = new Mock<Solution2>();
            string projectName = "testProject";
            string rosAttributeProjectName = "testRosAttributeProjectName";
            string projectTemplate = "testProjectTemplate";

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()))
                .Returns(dteMock.Object);
            solutionMock.As<Solution>();
            dteMock.SetupGet(dte => dte.Solution).Returns((Solution)solutionMock.Object);

            //act
            SolutionManager testClass = new SolutionManager(serviceProviderMock.Object, projectName, rosAttributeProjectName, projectTemplate);

            //assert
            testClass._solution.Should().BeSameAs(solutionMock.Object);
            testClass._projectName.Should().Be(projectName);
            testClass._rosMessageTypeAttributeProjectName.Should().Be(rosAttributeProjectName);
            testClass._projectTemplateAndFrameworkVersion.Should().Be(projectTemplate);

            serviceProviderMock.Verify(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()), Times.Once);
            dteMock.Verify(dte => dte.Solution, Times.Once);
        }

        [Test]
        public void Constructor_UnitTest_ServiceProviderIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            string projectName = "testProject";
            string rosAttributeProjectName = "testRosAttributeProjectName";
            string projectTemplate = "testProjectTemplate";

            //act
            Action act = () => new SolutionManager(null, projectName, rosAttributeProjectName, projectTemplate);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void Constructor_UnitTest_ProjectNameIsNull_ShouldThrowArgumentException()
        {
            //arrange
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            string projectName = null;
            string rosAttributeProjectName = "testRosAttributeProjectName";
            string projectTemplate = "testProjectTemplate";

            //act
            Action act = () => new SolutionManager(serviceProviderMock.Object, projectName, rosAttributeProjectName, projectTemplate);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void Constructor_UnitTest_ProjectNameIsEmpty_ShouldThrowArgumentException()
        {
            //arrange
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            string projectName = string.Empty;
            string rosAttributeProjectName = "testRosAttributeProjectName";
            string projectTemplate = "testProjectTemplate";

            //act
            Action act = () => new SolutionManager(serviceProviderMock.Object, projectName, rosAttributeProjectName, projectTemplate);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void Constructor_UnitTest_RosAttributeProjectNameIsNull_ShouldThrowArgumentException()
        {
            //arrange
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            string projectName = "testProject";
            string rosAttributeProjectName = null;
            string projectTemplate = "testProjectTemplate";

            //act
            Action act = () => new SolutionManager(serviceProviderMock.Object, projectName, rosAttributeProjectName, projectTemplate);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void Constructor_UnitTest_RosAttributeProjectNameIsEmpty_ShouldThrowArgumentException()
        {
            //arrange
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            string projectName = "testProject";
            string rosAttributeProjectName = string.Empty;
            string projectTemplate = "testProjectTemplate";

            //act
            Action act = () => new SolutionManager(serviceProviderMock.Object, projectName, rosAttributeProjectName, projectTemplate);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void Constructor_UnitTest_ProjectTemplateIsNull_ShouldThrowArgumentException()
        {
            //arrange
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            string projectName = "testProject";
            string rosAttributeProjectName = "testRosAttributeProjectName";
            string projectTemplate = null;

            //act
            Action act = () => new SolutionManager(serviceProviderMock.Object, projectName, rosAttributeProjectName, projectTemplate);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void Constructor_UnitTest_ProjectTemplateIsEmpty_ShouldThrowArgumentException()
        {
            //arrange
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            string projectName = "testProject";
            string rosAttributeProjectName = "testRosAttributeProjectName";
            string projectTemplate = string.Empty;

            //act
            Action act = () => new SolutionManager(serviceProviderMock.Object, projectName, rosAttributeProjectName, projectTemplate);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}
