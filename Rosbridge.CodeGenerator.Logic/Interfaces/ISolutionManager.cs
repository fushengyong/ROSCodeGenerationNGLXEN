namespace Rosbridge.CodeGenerator.Logic.Interfaces
{
    using EnvDTE;

    public interface ISolutionManager
    {
        void Initialize();
        ProjectItem AddFileToProjectItem(ProjectItem projectItem, string filePath);
        ProjectItem AddNewDirectoryToProject(string directoryName);
        string GetProjectItemFullPath(ProjectItem projectItem);
    }
}
