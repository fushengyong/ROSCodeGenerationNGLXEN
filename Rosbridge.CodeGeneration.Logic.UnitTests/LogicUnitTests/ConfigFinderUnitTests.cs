namespace Rosbridge.CodeGeneration.Logic.UnitTests.LogicUnitTests
{
    using EnvDTE;
    using FluentAssertions;
    using Helpers;
    using Moq;
    using NUnit.Framework;
    using System;
    using System.IO;

    [TestFixture]
    public class ConfigFinderUnitTests
    {

        [Test]
        public void GetConfigFilePath_ShouldReturnConfigFile()
        {
            //arrange
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            Mock<DTE> dteMock = new Mock<DTE>();
            Mock<Solution> solutionMock = new Mock<Solution>();
            Mock<ProjectItem> projectProjectItemMock = new Mock<ProjectItem>();
            Mock<Project> projectMock = new Mock<Project>();
            Mock<ProjectItems> projectItemsMock = new Mock<ProjectItems>();
            Mock<ProjectItem> configFileProjectItemMock = new Mock<ProjectItem>();

            string configFileName = "app.config";
            string configFilePath = "testConfigFilePath";
            string templateFile = "testTemplateFilePath";

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()))
                .Returns(dteMock.Object);
            dteMock.SetupGet(dte => dte.Solution).Returns(solutionMock.Object);
            solutionMock.Setup(solution => solution.FindProjectItem(templateFile)).Returns(projectProjectItemMock.Object);
            projectProjectItemMock.SetupGet(projectItem => projectItem.ContainingProject).Returns(projectMock.Object);
            projectMock.SetupGet(project => project.ProjectItems).Returns(projectItemsMock.Object);
            projectItemsMock.Setup(projectItems => projectItems.GetEnumerator())
                .Returns(new ProjectItem[] { configFileProjectItemMock.Object }.GetEnumerator());
            configFileProjectItemMock.SetupGet(configFileProjectItem => configFileProjectItem.Name).Returns(configFileName);
            configFileProjectItemMock.Setup(configFileProjectItem => configFileProjectItem.get_FileNames(0))
                .Returns(configFilePath);

            //act
            string configFileResult = ConfigFinder.GetConfigFilePath(serviceProviderMock.Object, templateFile);

            //assert
            configFileResult.Should().NotBeNull();
            configFileResult.Should().Be(configFilePath);
        }

        [Test]
        public void GetConfigFilePath_ConfigFileNotFound_ShouldThrowFileNotFoundException()
        {
            //arrange
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            Mock<DTE> dteMock = new Mock<DTE>();
            Mock<Solution> solutionMock = new Mock<Solution>();
            Mock<ProjectItem> projectProjectItemMock = new Mock<ProjectItem>();
            Mock<Project> projectMock = new Mock<Project>();
            Mock<ProjectItems> projectItemsMock = new Mock<ProjectItems>();
            Mock<ProjectItem> configFileProjectItemMock = new Mock<ProjectItem>();

            string configFileName = "wrongFileName";
            string configFilePath = "testConfigFilePath";
            string templateFile = "testTemplateFilePath";

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(It.IsAny<Type>()))
                .Returns(dteMock.Object);
            dteMock.SetupGet(dte => dte.Solution).Returns(solutionMock.Object);
            solutionMock.Setup(solution => solution.FindProjectItem(templateFile)).Returns(projectProjectItemMock.Object);
            projectProjectItemMock.SetupGet(projectItem => projectItem.ContainingProject).Returns(projectMock.Object);
            projectMock.SetupGet(project => project.ProjectItems).Returns(projectItemsMock.Object);
            projectItemsMock.Setup(projectItems => projectItems.GetEnumerator())
                .Returns(new ProjectItem[] { configFileProjectItemMock.Object }.GetEnumerator());
            configFileProjectItemMock.SetupGet(configFileProjectItem => configFileProjectItem.Name).Returns(configFileName);
            configFileProjectItemMock.Setup(configFileProjectItem => configFileProjectItem.get_FileNames(0))
                .Returns(configFilePath);

            //act
            Action act = () => ConfigFinder.GetConfigFilePath(serviceProviderMock.Object, templateFile);

            //assert
            act.Should().ThrowExactly<FileNotFoundException>();
        }

        [Test]
        public void GetConfigFilePath_ServiceProviderIsNull_ShouldThrowArgumentNullException()
        {
            //arrange
            string templateFile = "testTemplateFile";

            //act
            Action act = () => ConfigFinder.GetConfigFilePath(null, templateFile);

            //assert
            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Test]
        public void GetConfigFilePath_TemplateFileIsNull_ShouldThrowArgumentException()
        {
            //arrange
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            string templateFile = null;

            //act
            Action act = () => ConfigFinder.GetConfigFilePath(serviceProviderMock.Object, templateFile);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Test]
        public void GetConfigFilePath_TemplateFileIsEmpty_ShouldThrowArgumentException()
        {
            //arrange
            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            string templateFile = string.Empty;

            //act
            Action act = () => ConfigFinder.GetConfigFilePath(serviceProviderMock.Object, templateFile);

            //assert
            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}
