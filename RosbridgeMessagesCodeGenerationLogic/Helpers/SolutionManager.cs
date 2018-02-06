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
        private const string PROJECT_TEMPLATE_PATH = "csClassLibrary.vstemplate|FrameworkVersion=4.5.2";
        private const string PROJECT_LANGUAGE = "CSharp";

        private Solution2 _solution;
        private Project _project;
        private string _projectName;
        private string _clientProjectName;

        public SolutionManager(IServiceProvider serviceProvider, string projectName, string clientProjectName)
        {
            if (null == serviceProvider)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (string.IsNullOrWhiteSpace(projectName))
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(projectName));
            }

            if (string.IsNullOrWhiteSpace(clientProjectName))
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(clientProjectName));
            }

            DTE dte = (DTE)serviceProvider.GetService(typeof(DTE));
            _solution = (Solution2)dte.Solution;
            _projectName = projectName;
            _clientProjectName = clientProjectName;
        }

        public void Initialize()
        {
            Project currentProject = GetProjectByName(_projectName);

            if (currentProject != null)
            {
                _solution.Remove(currentProject);
                Directory.Delete(Path.GetDirectoryName(currentProject.FullName), true);
            }

            string classLibraryTemplatePath = _solution.GetProjectTemplate(PROJECT_TEMPLATE_PATH, PROJECT_LANGUAGE);
            string solutionPath = Path.GetDirectoryName(_solution.FullName);

            _solution.AddFromTemplate(classLibraryTemplatePath, Path.Combine(solutionPath, _projectName), _projectName);

            Project newProject = GetProjectByName(_projectName);

            Project clientProject = GetProjectByName(_clientProjectName);

            VSProject newProjectVSProj = newProject.Object;
            newProjectVSProj.References.AddProject(clientProject);

            _project = newProject;
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

        public ProjectItem AddFileToProjectItem(ProjectItem projectItem, FileInfo file)
        {
            if (null == projectItem)
            {
                throw new ArgumentNullException(nameof(projectItem));
            }

            if (null == file)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException(file.FullName);
            }

            if (projectItem.Kind != PROJECT_DIRECTORY_GUID)
            {
                throw new InvalidOperationException("The given project item is not a directory!");
            }

            return projectItem.ProjectItems.AddFromFile(file.FullName);
        }

        public ProjectItem AddDirectoryToProject(string directoryName)
        {
            if (string.IsNullOrWhiteSpace(directoryName))
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
