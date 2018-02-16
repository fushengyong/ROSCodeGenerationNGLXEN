namespace Rosbridge.CodeGenerator.Logic.BaseClasses
{
    using Rosbridge.CodeGenerator.Logic.Enums;
    using Rosbridge.CodeGenerator.Logic.Interfaces;
    using System;
    using System.IO;
    using System.Linq;

    public class SrvFile : RosFile
    {
        public const string FILE_EXTENSION = "srv";

        private const string INPUT_OUTPUT_SEPARATOR = "---";
        private const string REQUEST_NAME = "Request";
        private const string RESPONSE_NAME = "Response";
        private const string TYPE_SEPARATOR = "__";

        public MsgFile Response { get; private set; }
        public MsgFile Request { get; private set; }

        private IYAMLParser _yamlParser;

        public SrvFile(IYAMLParser yamlParser, FileInfo file) : base(file)
        {
            _yamlParser = yamlParser;
            ProcessFields();
        }

        protected override void ProcessFields()
        {
            string[] splittedFileContent = this.FileContent.Split(new string[] { INPUT_OUTPUT_SEPARATOR }, StringSplitOptions.None);

            string requestString = splittedFileContent.FirstOrDefault();
            string responseString = splittedFileContent.LastOrDefault();

            if (requestString != null)
            {
                string tempClassName = $"{this.Type.Type}{TYPE_SEPARATOR}{RESPONSE_NAME}";
                Request = new MsgFile(_yamlParser, requestString, tempClassName, this.Type.Namespace, ServiceMessageTypeEnum.Request);
            }

            if (responseString != null)
            {
                string tempClassName = $"{this.Type.Type}{TYPE_SEPARATOR}{REQUEST_NAME}";
                Response = new MsgFile(_yamlParser, responseString, tempClassName, this.Type.Namespace, ServiceMessageTypeEnum.Response);
            }
        }
    }
}
