namespace Rosbridge.CodeGeneration.Logic.BaseClasses
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// DTO base for a ROS file (.msg, .srv)
    /// </summary>
    public abstract class RosFile
    {
        private readonly string[] RosPackageFolderNameArray = { "msg", "srv", "msgs", "srvs" };

        /// <summary>
        /// ROS file FileInfo
        /// </summary>
        public FileInfo RosFileInfo { get; private set; }
        /// <summary>
        /// ROS file package DirectoryInfo
        /// </summary>
        public DirectoryInfo PackageDirectoryInfo { get; private set; }
        /// <summary>
        /// ROS file filcontent. In YAML format
        /// </summary>
        public string FileContent { get; private set; }
        /// <summary>
        /// ROS file type
        /// </summary>
        public MessageType Type { get; private set; }

        public RosFile(FileInfo file)
        {
            if (null == file)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException(file.FullName);
            }

            this.RosFileInfo = file;
            this.FileContent = File.ReadAllText(this.RosFileInfo.FullName);
            SetPackageDirectoryInfo();
            string namespaceName = this.PackageDirectoryInfo.Name;
            string className = Path.GetFileNameWithoutExtension(this.RosFileInfo.Name);
            Type = new MessageType(namespaceName, className);
        }

        public RosFile(string fileContent, string className, string namespaceName)
        {
            if (null == fileContent)
            {
                throw new ArgumentNullException(nameof(fileContent));
            }

            if (null == className)
            {
                throw new ArgumentNullException(nameof(className));
            }

            if (null == namespaceName)
            {
                throw new ArgumentNullException(nameof(namespaceName));
            }

            //FileContent can be empty (service with no response)

            if (string.Empty == className)
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(className));
            }

            if (string.Empty == namespaceName)
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(namespaceName));
            }

            this.FileContent = fileContent;
            Type = new MessageType(namespaceName, className);
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is RosFile)
            {

                RosFile item = obj as RosFile;

                if (item == null)
                {
                    return false;
                }

                return this.Type == item.Type;
            }
            return false;
        }

        /// <summary>
        /// Process the ROS file content
        /// </summary>
        protected abstract void ProcessFields();

        private void SetPackageDirectoryInfo()
        {
            if (RosPackageFolderNameArray.Contains(this.RosFileInfo.Directory.Name))
            {
                this.PackageDirectoryInfo = this.RosFileInfo.Directory.Parent;
            }
            else
            {
                this.PackageDirectoryInfo = this.RosFileInfo.Directory;
            }
        }

        public override int GetHashCode()
        {
            return 2049151605 + EqualityComparer<MessageType>.Default.GetHashCode(Type);
        }
    }
}
