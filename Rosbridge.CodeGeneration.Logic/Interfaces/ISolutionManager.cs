namespace Rosbridge.CodeGeneration.Logic.Interfaces
{
    using EnvDTE;

    public interface ISolutionManager
    {
        void Initialize();
        ProjectItem AddFileToDirectoryProjectItem(ProjectItem projectItem, string filePath);
        ProjectItem AddNewDirectoryToProject(string newDirectoryName);
        string GetProjectItemFullPath(ProjectItem projectItem);
    }
}
