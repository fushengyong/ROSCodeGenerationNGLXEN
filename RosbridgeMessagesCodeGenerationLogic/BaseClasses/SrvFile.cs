namespace RosbridgeMessagesCodeGenerationLogic.BaseClasses
{
    using RosbridgeMessagesCodeGenerationLogic.Enums;
    using RosbridgeMessagesCodeGenerationLogic.Interfaces;
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

        public SrvFile(FileInfo file, IYAMLParser yamlParser) : base(file)
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
                string tempClassName = string.Format("{0}{1}{2}", this.Type.TypeName, TYPE_SEPARATOR, RESPONSE_NAME);
                Request = new MsgFile(requestString, tempClassName, this.Type.NamespaceName, ServiceMessageTypeEnum.Request, _yamlParser);
            }

            if (responseString != null)
            {
                string tempClassName = string.Format("{0}{1}{2}", this.Type.TypeName, TYPE_SEPARATOR, REQUEST_NAME);
                Response = new MsgFile(responseString, tempClassName, this.Type.NamespaceName, ServiceMessageTypeEnum.Response, _yamlParser);
            }
        }
    }
}
