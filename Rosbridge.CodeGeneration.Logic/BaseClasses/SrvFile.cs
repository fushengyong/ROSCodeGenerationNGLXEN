namespace Rosbridge.CodeGeneration.Logic.BaseClasses
{
    using Rosbridge.CodeGeneration.Logic.Interfaces;
    using System;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// DTO for an .srv file
    /// </summary>
    public class SrvFile : RosFile
    {
        private const string INPUT_OUTPUT_SEPARATOR = "---";
        private const string REQUEST_NAME = "Request";
        private const string RESPONSE_NAME = "Response";
        private const string TYPE_SEPARATOR = "__";

        /// <summary>
        /// Service request message
        /// </summary>
        public MsgFile Request { get; private set; }
        /// <summary>
        /// Service response message
        /// </summary>
        public MsgFile Response { get; private set; }

        /// <summary>
        /// YAML parser
        /// </summary>
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
                Request = new MsgFile(_yamlParser, requestString, tempClassName, this.Type.Namespace);
            }

            if (responseString != null)
            {
                string tempClassName = $"{this.Type.Type}{TYPE_SEPARATOR}{REQUEST_NAME}";
                Response = new MsgFile(_yamlParser, responseString, tempClassName, this.Type.Namespace);
            }
        }
    }
}
