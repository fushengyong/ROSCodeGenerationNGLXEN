namespace Rosbridge.CodeGeneration.Logic.BaseClasses
{
    using Rosbridge.CodeGeneration.Logic.Constants;
    using Rosbridge.CodeGeneration.Logic.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class MsgFile : RosFile
    {
        private static string standardNamespaceValue;
        public static string StandardNamespace
        {
            get { return standardNamespaceValue; }
        }

        private IYAMLParser _yamlParser;

        public ISet<MessageType> DependencySet { get; private set; }
        public ISet<MessageField> FieldSet { get; private set; }
        public ISet<MessageField> ConstantFieldSet { get; private set; }
        public ISet<MessageField> ArrayFieldSet { get; private set; }

        public MsgFile(IYAMLParser yamlParser, FileInfo file) : base(file)
        {
            if (null == yamlParser)
            {
                throw new ArgumentNullException(nameof(yamlParser));
            }

            this._yamlParser = yamlParser;
            this.FieldSet = new HashSet<MessageField>();
            this.ConstantFieldSet = new HashSet<MessageField>();
            this.ArrayFieldSet = new HashSet<MessageField>();
            this.DependencySet = new HashSet<MessageType>();
            ProcessFields();
        }

        public MsgFile(IYAMLParser yamlParser, string fileContent, string className, string namespaceValue) : base(fileContent, className, namespaceValue)
        {
            if (null == yamlParser)
            {
                throw new ArgumentNullException(nameof(yamlParser));
            }

            this._yamlParser = yamlParser;
            this.FieldSet = new HashSet<MessageField>();
            this.ConstantFieldSet = new HashSet<MessageField>();
            this.ArrayFieldSet = new HashSet<MessageField>();
            this.DependencySet = new HashSet<MessageType>();
            ProcessFields();
        }

        protected override void ProcessFields()
        {
            _yamlParser.SetMsgFileFieldsFromYAMLString(this.FileContent, this);

            AddDependencies(this.FieldSet);
            AddDependencies(this.ArrayFieldSet);
            AddDependencies(this.ConstantFieldSet);

            if (this.Type.Type.ToLower() == RosConstants.MessageTypes.HEADER_TYPE.ToLower())
            {
                standardNamespaceValue = this.Type.Namespace;
            }
        }

        private void AddDependencies(ISet<MessageField> fieldSet)
        {
            foreach (MessageField field in fieldSet)
            {
                if (this.Type.Type == field.Type.Type && RosConstants.MessageTypes.CustomTimePrimitiveTypeSet.Contains(this.Type.Type))
                {
                    field.Type.Type = RosConstants.MessageTypes.CUSTOM_TIME_PRIMITIVE_TYPE;
                }

                if (!string.IsNullOrEmpty(field.Type.Namespace) && this.Type.Namespace != field.Type.Namespace)
                {
                    DependencySet.Add(new MessageType(field.Type.Namespace, field.Type.Type));
                }
            }
        }
    }
}
