namespace Rosbridge.CodeGenerator.Logic.Helpers
{
    using Rosbridge.CodeGenerator.Logic.BaseClasses;
    using Rosbridge.CodeGenerator.Logic.Interfaces;
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

        public void LoadRosFiles(ISet<MsgFile> msgFileSet, ISet<SrvFile> srvFileSet, string path)
        {
            if (null == msgFileSet)
            {
                throw new ArgumentNullException(nameof(msgFileSet));
            }

            if (null == srvFileSet)
            {
                throw new ArgumentNullException(nameof(srvFileSet));
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
                    string.Format("*.{0}", MsgFile.FILE_EXTENSION),
                    SearchOption.AllDirectories
                ).Select(filePath => new FileInfo(filePath)));

            ISet<FileInfo> srvFileInfoSet = new HashSet<FileInfo>(
                Directory.GetFiles(
                    path,
                    string.Format("*.{0}", SrvFile.FILE_EXTENSION),
                    SearchOption.AllDirectories
                ).Select(filePath => new FileInfo(filePath)));

            msgFileSet.UnionWith(msgFileInfoSet.Select(FileInfo => new MsgFile(FileInfo, _yamlParser)));
            srvFileSet.UnionWith(srvFileInfoSet.Select(FileInfo => new SrvFile(FileInfo, _yamlParser)));
        }
    }
}
