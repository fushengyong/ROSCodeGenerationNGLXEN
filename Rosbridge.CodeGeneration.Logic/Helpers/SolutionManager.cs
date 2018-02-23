﻿namespace Rosbridge.CodeGeneration.Logic.Helpers
{
    using EnvDTE;
    using EnvDTE80;
    using Rosbridge.CodeGeneration.Logic.Exceptions;
    using Rosbridge.CodeGeneration.Logic.Interfaces;
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.InteropServices;
    using VSLangProj;

    /// <summary>
    /// SolutionManager for solution file and folder operations
    /// </summary>
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
                try
                {
                    _solution.Remove(currentProject);
                }
                catch (COMException e) { Debug.WriteLine(e.Message); }
            }

            string solutionPath = Path.GetDirectoryName(_solution.FullName);
            string projectPath = Path.Combine(solutionPath, _projectName);

            TryDeleteDirectory(projectPath);

            string classLibraryTemplatePath = _solution.GetProjectTemplate(_projectTemplateAndFrameworkVersion, PROJECT_LANGUAGE);

            _solution.AddFromTemplate(classLibraryTemplatePath, projectPath, _projectName);

            Project newProject = GetProjectByName(_projectName);

            DeleteDefaultClass(newProject);

            Project clientProject = GetProjectByName(_rosMessageTypeAttributeProjectName);

            if (null != clientProject)
            {
                VSProject newProjectVSProj = (VSProject)newProject.Object;
                newProjectVSProj.References.AddProject(clientProject);
            }
            else
            {
                throw new ProjectNotFoundException($"There is no such project: {_rosMessageTypeAttributeProjectName}");
            }

            _project = newProject;
        }

        public ProjectItem AddFileToDirectoryProjectItem(ProjectItem projectItem, string filePath)
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

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found!", filePath);
            }

            return projectItem.ProjectItems.AddFromFile(filePath);
        }

        public ProjectItem AddNewDirectoryToProject(string newDirectoryName)
        {
            if (null == _project)
            {
                throw new InvalidOperationException("Project not initialized yet!");
            }

            if (null == newDirectoryName)
            {
                throw new ArgumentNullException(nameof(newDirectoryName));
            }

            if (string.Empty == newDirectoryName)
            {
                throw new ArgumentException("Property cannot be empty!", nameof(newDirectoryName));
            }

            TryDeleteDirectory(Path.Combine(Path.GetDirectoryName(_project.FullName), newDirectoryName));

            return _project.ProjectItems.AddFolder(newDirectoryName);
        }

        public string GetProjectItemFullPath(ProjectItem projectItem)
        {
            return projectItem.Properties.Item(FULL_PATH_ITEM_PROPERTY).Value.ToString();
        }

        private void TryDeleteDirectory(string directoryPath, bool recursive = true)
        {
            try
            {
                Directory.Delete(directoryPath, recursive);
            }
            catch (DirectoryNotFoundException e) { Debug.WriteLine(e.Message); }
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
                Project resultProject = GetProjectByNameRecursively(project, projectName);
                if (null != resultProject)
                {
                    return resultProject;
                }
            }

            return null;
        }

        private Project GetProjectByNameRecursively(Project solutionProject, string projectName)
        {
            if (solutionProject.Kind == ProjectKinds.vsProjectKindSolutionFolder)
            {
                foreach (ProjectItem projectItem in solutionProject.ProjectItems)
                {
                    if (null != projectItem.SubProject)
                    {
                        Project tmpProject = projectItem.SubProject as Project;
                        Project resultProject = GetProjectByNameRecursively(tmpProject, projectName);
                        if (null != resultProject)
                        {
                            return resultProject;
                        }
                    }
                }
            }
            else
            {
                if (solutionProject.Name == projectName)
                {
                    return solutionProject;
                }
            }

            return null;
        }
    }
}