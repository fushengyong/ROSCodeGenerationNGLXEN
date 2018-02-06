namespace RosbridgeMessagesCodeGenerationLogic.Helpers
{
    using EnvDTE;
    using EnvDTE80;
    using RosbridgeMessagesCodeGenerationLogic.Interfaces;
    using System;
    using System.IO;
    using VSLangProj;

    public class SolutionManager : ISolutionManager
    {
        private const string PROJECT_DIRECTORY_GUID = "{6BB5F8EF-4483-11D3-8BCF-00C04F8EC28C}";
        private const string FULL_PATH_ITEM_PROPERTY = "FullPath";
        private const string PROJECT_LANGUAGE = "CSharp";
        private const string DEFAULT_CLASS_NAME = "Class1.cs";

        private Solution2 _solution;
        private Project _project;
        private string _projectName;
        private string _projectTemplateAndFrameworkVersion;
        private string _rosMessageTypeAttributeProjectName;

        public SolutionManager(IServiceProvider serviceProvider, string projectName, string rosMessageTypeAttributeProjectName, string projectTemplateAndFrameworkVersion)
        {
            if (null == serviceProvider)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (null == projectName)
            {
                throw new ArgumentNullException(nameof(projectName));
            }

            if (string.Empty == projectName)
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(projectName));
            }

            if (null == rosMessageTypeAttributeProjectName)
            {
                throw new ArgumentNullException(nameof(rosMessageTypeAttributeProjectName));
            }

            if (string.Empty == rosMessageTypeAttributeProjectName)
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(rosMessageTypeAttributeProjectName));
            }

            if (null == projectTemplateAndFrameworkVersion)
            {
                throw new ArgumentNullException(nameof(projectTemplateAndFrameworkVersion));
            }

            if (string.Empty == projectTemplateAndFrameworkVersion)
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(projectTemplateAndFrameworkVersion));
            }

            DTE dte = (DTE)serviceProvider.GetService(typeof(DTE));
            _solution = (Solution2)dte.Solution;
            _projectName = projectName;
            _rosMessageTypeAttributeProjectName = rosMessageTypeAttributeProjectName;
            _projectTemplateAndFrameworkVersion = projectTemplateAndFrameworkVersion;
        }

        public void Initialize()
        {
            Project currentProject = GetProjectByName(_projectName);

            if (currentProject != null)
            {
                Directory.Delete(Path.GetDirectoryName(currentProject.FullName), true);
                _solution.Remove(currentProject);
            }

            string classLibraryTemplatePath = _solution.GetProjectTemplate(_projectTemplateAndFrameworkVersion, PROJECT_LANGUAGE);
            string solutionPath = Path.GetDirectoryName(_solution.FullName);

            _solution.AddFromTemplate(classLibraryTemplatePath, Path.Combine(solutionPath, _projectName), _projectName);

            Project newProject = GetProjectByName(_projectName);

            DeleteDefaultClass(newProject);

            Project clientProject = GetProjectByName(_rosMessageTypeAttributeProjectName);

            VSProject newProjectVSProj = newProject.Object;
            newProjectVSProj.References.AddProject(clientProject);

            _project = newProject;
        }

        private void DeleteDefaultClass(Project project)
        {
            foreach (ProjectItem projectItem in project.ProjectItems)
            {
                if (projectItem.Name == DEFAULT_CLASS_NAME)
                {
                    File.Delete(GetProjectItemFullPath(projectItem));
                    projectItem.Remove();
                }
            }
        }

        private Project GetProjectByName(string projectName)
        {
            foreach (Project project in _solution.Projects)
            {
                if (project.Name == projectName)
                {
                    return project;
                }
            }

            return null;
        }

        public ProjectItem AddFileToProjectItem(ProjectItem projectItem, string filePath)
        {
            if (null == projectItem)
            {
                throw new ArgumentNullException(nameof(projectItem));
            }

            if (null == filePath)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            if (string.Empty == filePath)
            {
                throw new ArgumentException("Property cannot be empty!", nameof(filePath));
            }

            if (projectItem.Kind != PROJECT_DIRECTORY_GUID)
            {
                throw new InvalidOperationException("The given project item is not a directory!");
            }

            return projectItem.ProjectItems.AddFromFile(filePath);
        }

        public ProjectItem AddDirectoryToProject(string directoryName)
        {
            if (null == _project)
            {
                throw new InvalidOperationException("Project not initialized yet!");
            }

            if (null == directoryName)
            {
                throw new ArgumentNullException(nameof(directoryName));
            }

            if (string.Empty == directoryName)
            {
                throw new ArgumentException("Property cannot be empty!", nameof(directoryName));
            }

            return _project.ProjectItems.AddFolder(directoryName);
        }

        public string GetProjectItemFullPath(ProjectItem projectItem)
        {
            return projectItem.Properties.Item(FULL_PATH_ITEM_PROPERTY).Value.ToString();
        }
    }
}
