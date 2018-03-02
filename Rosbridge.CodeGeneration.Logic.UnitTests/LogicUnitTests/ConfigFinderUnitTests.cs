namespace Rosbridge.CodeGeneration.Logic.UnitTests.LogicUnitTests
{
    using EnvDTE;
    using FluentAssertions;
    using Helpers;
    using Moq;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class ConfigFinderUnitTests
    {
        [Test]
        public void GetConfigFilePath_UnitTest_ArgumentsOK_ShouldReturnConfigFilePath()
        {
            //arrange
            string configFileName = "app.config";
            string configFilePath = "testPath";

            Mock<IServiceProvider> serviceProviderMock = new Mock<IServiceProvider>();
            string templateFilePath = "testPath";

            Mock<DTE> dteMock = new Mock<DTE>();
            Mock<Solution> solutionMock = new Mock<Solution>();

            Mock<ProjectItem> projectItemMock = new Mock<ProjectItem>();
            Mock<Project> projectMock = new Mock<Project>();
            Mock<ProjectItems> projectProjectItemsMock = new Mock<ProjectItems>();
            Mock<ProjectItem> configFileProjectItemMock = new Mock<ProjectItem>();

            serviceProviderMock.Setup(serviceProvider => serviceProvider.GetService(typeof(DTE)))
                .Returns(dteMock.Object);
            dteMock.SetupGet(dte => dte.Solution).Returns(solutionMock.Object);
            solutionMock.Setup(solution => solution.FindProjectItem(templateFilePath)).Returns(projectItemMock.Object);
            projectItemMock.SetupGet(projectItem => projectItem.ContainingProject).Returns(projectMock.Object);
            projectMock.SetupGet(project => project.ProjectItems)
                .Returns(projectProjectItemsMock.Object);
            projectProjectItemsMock.Setup(projectItems => projectItems.GetEnumerator()).Returns(new[] { configFileProjectItemMock.Object }.GetEnumerator());
            configFileProjectItemMock.SetupGet(configProjectItem => configProjectItem.Name).Returns(configFileName);
            configFileProjectItemMock.SetupGet(configProjectItem => configProjectItem.get_FileNames(0)).Returns(configFilePath);

            //act
            string resultConfigFilePath = ConfigFinder.GetConfigFilePath(serviceProviderMock.Object, templateFilePath);

            //assert
            resultConfigFilePath.Should().NotBeNull();
            resultConfigFilePath.Should().Be(configFilePath);
        }
    }
}
