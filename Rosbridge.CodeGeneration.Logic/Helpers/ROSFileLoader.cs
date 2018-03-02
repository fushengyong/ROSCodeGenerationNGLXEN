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
    public class ROSFileLoader
    {
        private IYAMLParser _yamlParser;

        public ROSFileLoader(IYAMLParser yamlParser)
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
        public void LoadRosFiles(ISet<IMsgFile> messageFileSet, ISet<ISrvFile> serviceFileSet, string directoryPath)
        {
            if (null == messageFileSet)
            {
                throw new ArgumentNullException(nameof(messageFileSet));
            }

            if (null == serviceFileSet)
            {
                throw new ArgumentNullException(nameof(serviceFileSet));
            }

            if (string.IsNullOrWhiteSpace(directoryPath))
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(directoryPath));
            }

            if (!IsDirectoryExists(directoryPath))
            {
                throw new DirectoryNotFoundException(directoryPath);
            }

            ISet<FileInfo> msgFileInfoSet = LoadFiles(directoryPath, RosConstants.FileExtensions.MSG_FILE_EXTENSION,
                SearchOption.AllDirectories);

            ISet<FileInfo> srvFileInfoSet = LoadFiles(directoryPath, RosConstants.FileExtensions.SRV_FILE_EXTENSION,
                SearchOption.AllDirectories);

            messageFileSet.UnionWith(CreateMessageFileCollection(msgFileInfoSet));
            serviceFileSet.UnionWith(CreateServiceFileCollection(srvFileInfoSet));
        }

        protected internal virtual bool IsDirectoryExists(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }

        protected internal virtual ISet<FileInfo> LoadFiles(string directoryPath, string fileExtension, SearchOption searchOption)
        {
            return new HashSet<FileInfo>(Directory.GetFiles(
                    directoryPath,
                    $"*.{fileExtension}",
                    searchOption)
                .Select(filePath => new FileInfo(filePath)));
        }

        protected internal virtual IEnumerable<IMsgFile> CreateMessageFileCollection(ISet<FileInfo> messageFileInfoSet)
        {
            return messageFileInfoSet.Select(fileInfo => new MsgFile(_yamlParser, fileInfo));
        }

        protected internal virtual IEnumerable<ISrvFile> CreateServiceFileCollection(ISet<FileInfo> serviceFileInfoSet)
        {
            return serviceFileInfoSet.Select(fileInfo => new SrvFile(_yamlParser, fileInfo));
        }
    }
}