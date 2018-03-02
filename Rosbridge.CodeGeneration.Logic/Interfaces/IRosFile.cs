namespace Rosbridge.CodeGeneration.Logic.Interfaces
{
    using Rosbridge.CodeGeneration.Logic.BaseClasses;
    using System.IO;

    public interface IRosFile
    {
        /// <summary>
        /// ROS file FileInfo
        /// </summary>
        FileInfo RosFileInfo { get; }
        /// <summary>
        /// ROS file package DirectoryInfo
        /// </summary>
        DirectoryInfo PackageDirectoryInfo { get; }
        /// <summary>
        /// ROS file content. In YAML format
        /// </summary>
        string FileContent { get; }
        /// <summary>
        /// ROS file type
        /// </summary>
        RosType Type { get; }
    }
}
