namespace RosbridgeMessagesCodeGenerationLogic.Interfaces
{
    using EnvDTE;
    using System.IO;

    public interface ISolutionManager
    {
        void Initialize();
        ProjectItem AddFileToProjectItem(ProjectItem projectItem, FileInfo file);
        ProjectItem AddDirectoryToProject(string directoryName);
        string GetProjectItemFullPath(ProjectItem projectItem);
    }
}
