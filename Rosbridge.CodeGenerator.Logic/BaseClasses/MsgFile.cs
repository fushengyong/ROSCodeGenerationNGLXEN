namespace Rosbridge.CodeGenerator.Logic.BaseClasses
{
    using Rosbridge.CodeGenerator.Logic.Enums;
    using Rosbridge.CodeGenerator.Logic.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class MsgFile : RosFile
    {
        public const string FILE_EXTENSION = "msg";
        public const string HEADER_TYPE = "Header";
        public const string CUSTOM_TIME_PRIMITIVE_TYPE = "TimeData";

        public static readonly Dictionary<string, string> PrimitiveTypeDictionary = new Dictionary<string, string> {
            {"float64", "double"},
            {"uint64", "ulong"},
            {"uint32", "uint"},
            {"uint16", "ushort"},
            {"uint8", "byte"},
            {"int64", "long"},
            {"int32", "int"},
            {"int16", "short"},
            {"int8", "sbyte"},
            {"byte", "byte"},
            {"bool", "bool"},
            {"char", "char"},
            {"string", "string"},
            {"float32", "Single"},
            {"time", "Time"},
            {"duration", "Duration"}
        };
        public static readonly ISet<MessageType> MessageTypeSet = new HashSet<MessageType>();

        private static readonly ISet<string> CustomTimePrimitiveTypeSet = new HashSet<string> { "Time", "Duration" };

        private IYAMLParser _yamlParser;

        public ServiceMessageTypeEnum ServiceMessageType { get; private set; }
        public ISet<MessageField> FieldSet { get; private set; }
        public ISet<MessageField> ConstantFieldSet { get; private set; }
        public ISet<MessageField> ArrayFieldSet { get; private set; }
        public ISet<MessageType> DependencySet { get; private set; }

        public MsgFile(FileInfo file, IYAMLParser yamlParser) : base(file)
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

        public MsgFile(string fileContent, string className, string namespaceValue, ServiceMessageTypeEnum type, IYAMLParser yamlParser) : base(fileContent, className, namespaceValue)
        {
            if (null == yamlParser)
            {
                throw new ArgumentNullException(nameof(yamlParser));
            }

            this.ServiceMessageType = type;
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

            MessageTypeSet.Add(this.Type);
        }

        private void AddDependencies(ISet<MessageField> fieldSet)
        {
            foreach (MessageField field in fieldSet)
            {
                if (this.Type.TypeName == field.Type.TypeName && CustomTimePrimitiveTypeSet.Contains(this.Type.TypeName))
                {
                    field.Type.TypeName = CUSTOM_TIME_PRIMITIVE_TYPE;
                }

                if (!string.IsNullOrEmpty(field.Type.NamespaceName) && this.Type.NamespaceName != field.Type.NamespaceName)
                {
                    DependencySet.Add(new MessageType(field.Type.NamespaceName, field.Type.TypeName));
                }
            }
        }
    }
}
