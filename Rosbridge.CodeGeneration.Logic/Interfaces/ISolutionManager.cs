namespace Rosbridge.CodeGeneration.Logic.Interfaces
{
    using EnvDTE;

    /// <summary>
    /// SolutionManager for solution file and folder operations
    /// </summary>
    public interface ISolutionManager
    {
        /// <summary>
        /// Initialize the solution for code generation
        /// </summary>
        void Initialize();
        /// <summary>
        /// Add new file to a directory project item
        /// </summary>
        /// <param name="projectItem"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        ProjectItem AddFileToDirectoryProjectItem(ProjectItem projectItem, string filePath);
        /// <summary>
        /// Add new directory to the ROS messages project
        /// </summary>
        /// <param name="newDirectoryName"></param>
        /// <returns></returns>
        ProjectItem AddNewDirectoryToProject(string newDirectoryName);
        /// <summary>
        /// Get full path of the given ProjectItem
        /// </summary>
        /// <param name="projectItem"></param>
        /// <returns></returns>
        string GetProjectItemFullPath(ProjectItem projectItem);
    }
}
