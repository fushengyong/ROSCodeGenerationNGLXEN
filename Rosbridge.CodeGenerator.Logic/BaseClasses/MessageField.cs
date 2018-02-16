namespace Rosbridge.CodeGenerator.Logic.BaseClasses
{
    using System;
    using System.Collections.Generic;

    public class MessageField
    {
        public MessageType Type { get; private set; }
        public string FieldName { get; private set; }
        public string FieldValue { get; private set; }
        public int ArrayElementCount { get; private set; }
        public bool IsArray { get; private set; }
        public bool IsConst { get { return !string.IsNullOrWhiteSpace(FieldValue); } }

        public MessageField(string fieldName, string typeName, string namespaceName, bool isArray, int arrayElementCount, string fieldValue)
        {
            if (null == fieldName)
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            if (string.Empty == fieldName)
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(fieldName));
            }

            if (null == typeName)
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            if (string.Empty == typeName)
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(typeName));
            }

            if (null == namespaceName)
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            if (null == fieldValue)
            {
                throw new ArgumentNullException(nameof(fieldName));
            }

            this.Type = new MessageType(namespaceName, typeName);
            this.FieldName = fieldName;
            this.IsArray = isArray;
            this.ArrayElementCount = arrayElementCount;
            this.FieldValue = fieldValue;
        }

        public override bool Equals(object obj)
        {
            if (obj is MessageField)
            {
                MessageField anotherMember = obj as MessageField;
                return this.FieldName == anotherMember.FieldName;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return 956599492 + EqualityComparer<string>.Default.GetHashCode(FieldName);
        }
    }
}
