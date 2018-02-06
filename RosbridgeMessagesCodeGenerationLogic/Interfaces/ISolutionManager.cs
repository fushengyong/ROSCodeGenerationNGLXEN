namespace RosbridgeMessagesCodeGenerationLogic.Interfaces
{
    using EnvDTE;

    public interface ISolutionManager
    {
        void Initialize();
        ProjectItem AddFileToProjectItem(ProjectItem projectItem, string filePath);
        ProjectItem AddDirectoryToProject(string directoryName);
        string GetProjectItemFullPath(ProjectItem projectItem);
    }
}
