namespace Rosbridge.CodeGeneration.Logic.BaseClasses
{
    using Rosbridge.CodeGeneration.Logic.Constants;
    using Rosbridge.CodeGeneration.Logic.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// DTO for an .msg file
    /// </summary>
    public class MsgFile : RosFile, IMsgFile
    {
        /// <summary>
        /// Value of the standard namespace property
        /// </summary>
        private static string standardNamespaceValue;
        /// <summary>
        /// The standard namespace of the message files. (Where the HEADER type is located)
        /// </summary>
        public static string StandardNamespace
        {
            get { return standardNamespaceValue; }
        }

        /// <summary>
        /// YAML parser
        /// </summary>
        private IYAMLParser _yamlParser;

        /// <summary>
        /// Message dependeny collection
        /// </summary>
        public ISet<RosType> DependencySet { get; private set; }
        /// <summary>
        /// Message field collection
        /// </summary>
        public ISet<MessageField> FieldSet { get; private set; }
        /// <summary>
        /// Message constant field collection
        /// </summary>
        public ISet<MessageField> ConstantFieldSet { get; private set; }
        /// <summary>
        /// Message array field collection
        /// </summary>
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
            this.DependencySet = new HashSet<RosType>();
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
            this.DependencySet = new HashSet<RosType>();
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

        /// <summary>
        /// Collect dependencies from a field collection
        /// </summary>
        /// <param name="fieldSet"></param>
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
                    DependencySet.Add(new RosType(field.Type.Namespace, field.Type.Type));
                }
            }
        }
    }
}
