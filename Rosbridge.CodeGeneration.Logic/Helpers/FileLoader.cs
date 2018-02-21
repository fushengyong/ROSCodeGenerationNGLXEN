namespace Rosbridge.CodeGeneration.Logic.Helpers
{
    using Rosbridge.CodeGeneration.Logic.BaseClasses;
    using Rosbridge.CodeGeneration.Logic.Constants;
    using Rosbridge.CodeGeneration.Logic.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// FileLoader for load ROS files (.msg, .srv)
    /// </summary>
    public class FileLoader
    {
        private IYAMLParser _yamlParser;

        public FileLoader(IYAMLParser yamlParser)
        {
            if (null == yamlParser)
            {
                throw new ArgumentNullException(nameof(yamlParser));
            }

            _yamlParser = yamlParser;
        }

        /// <summary>
        /// Load .msg and .srv files from the given path
        /// </summary>
        /// <param name="messageFileSet"></param>
        /// <param name="serviceFileSet"></param>
        /// <param name="directoryPath"></param>
        public void LoadRosFiles(ISet<MsgFile> messageFileSet, ISet<SrvFile> serviceFileSet, string directoryPath)
        {
            if (null == messageFileSet)
            {
                throw new ArgumentNullException(nameof(messageFileSet));
            }

            if (null == serviceFileSet)
            {
                throw new ArgumentNullException(nameof(serviceFileSet));
            }

            if (null == directoryPath)
            {
                throw new ArgumentNullException(nameof(directoryPath));
            }

            if (string.Empty == directoryPath)
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(directoryPath));
            }

            if (!Directory.Exists(directoryPath))
            {
                throw new DirectoryNotFoundException(directoryPath);
            }

            ISet<FileInfo> msgFileInfoSet = new HashSet<FileInfo>(
                Directory.GetFiles(
                    directoryPath,
                    $"*.{RosConstants.FileExtensions.MSG_FILE_EXTENSION}",
                    SearchOption.AllDirectories
                ).Select(filePath => new FileInfo(filePath)));

            ISet<FileInfo> srvFileInfoSet = new HashSet<FileInfo>(
                Directory.GetFiles(
                    directoryPath,
                    $"*.{RosConstants.FileExtensions.MSG_FILE_EXTENSION}",
                    SearchOption.AllDirectories
                ).Select(filePath => new FileInfo(filePath)));

            messageFileSet.UnionWith(msgFileInfoSet.Select(FileInfo => new MsgFile(_yamlParser, FileInfo)));
            serviceFileSet.UnionWith(srvFileInfoSet.Select(FileInfo => new SrvFile(_yamlParser, FileInfo)));
        }
    }
}
