namespace Templates.BaseClasses
{
    using System;
    using System.IO;
    using System.Linq;

    internal sealed class SrvFile : RosFile
    {
        private const string INPUT_OUTPUT_SEPARATOR = "---";
        private const string REQUEST_NAME = "Request";
        private const string RESPONSE_NAME = "Response";
        private const string TYPE_SEPARATOR = "__";

        public const string FILE_EXTENSION = "srv";

        public MsgFile Response { get; private set; }
        public MsgFile Request { get; private set; }

        public SrvFile(FileInfo file) : base(file)
        {
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
                Request = new MsgFile(requestString, tempClassName, this.Type.NamespaceName, ServiceMessageTypeEnum.Request);
            }

            if (responseString != null)
            {
                string tempClassName = string.Format("{0}{1}{2}", this.Type.TypeName, TYPE_SEPARATOR, REQUEST_NAME);
                Response = new MsgFile(responseString, tempClassName, this.Type.NamespaceName, ServiceMessageTypeEnum.Response);
            }
        }
    }
}
