namespace Rosbridge.CodeGeneration.Logic.Helpers
{
    using Rosbridge.CodeGeneration.Logic.BaseClasses;
    using Rosbridge.CodeGeneration.Logic.Constants;
    using Rosbridge.CodeGeneration.Logic.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

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

        public void LoadRosFiles(ISet<MsgFile> messageFileSet, ISet<SrvFile> serviceFileSet, string path)
        {
            if (null == messageFileSet)
            {
                throw new ArgumentNullException(nameof(messageFileSet));
            }

            if (null == serviceFileSet)
            {
                throw new ArgumentNullException(nameof(serviceFileSet));
            }

            if (null == path)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (string.Empty == path)
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(path));
            }

            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException(path);
            }

            ISet<FileInfo> msgFileInfoSet = new HashSet<FileInfo>(
                Directory.GetFiles(
                    path,
                    $"*.{RosConstants.FileExtensions.MSG_FILE_EXTENSION}",
                    SearchOption.AllDirectories
                ).Select(filePath => new FileInfo(filePath)));

            ISet<FileInfo> srvFileInfoSet = new HashSet<FileInfo>(
                Directory.GetFiles(
                    path,
                    $"*.{RosConstants.FileExtensions.MSG_FILE_EXTENSION}",
                    SearchOption.AllDirectories
                ).Select(filePath => new FileInfo(filePath)));

            messageFileSet.UnionWith(msgFileInfoSet.Select(FileInfo => new MsgFile(_yamlParser, FileInfo)));
            serviceFileSet.UnionWith(srvFileInfoSet.Select(FileInfo => new SrvFile(_yamlParser, FileInfo)));
        }
    }
}
