namespace Rosbridge.CodeGeneration.Logic.BaseClasses
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// ROS message field class
    /// </summary>
    public class MessageField
    {
        /// <summary>
        /// Type of the message field
        /// </summary>
        public RosType Type { get; private set; }
        /// <summary>
        /// Field variable name
        /// </summary>
        public string FieldName { get; private set; }
        /// <summary>
        /// Field value, if the field is constant
        /// </summary>
        public string FieldValue { get; private set; }
        /// <summary>
        /// Field array size, if the field is an array
        /// </summary>
        public int ArrayElementCount { get; private set; }
        /// <summary>
        /// True if the field is an array
        /// </summary>
        public bool IsArray { get; private set; }
        /// <summary>
        /// True if the field has value
        /// </summary>
        public bool IsConst { get { return !string.IsNullOrWhiteSpace(FieldValue); } }

        public MessageField(string fieldName, string type, string @namespace, bool isArray, int arrayElementCount, string fieldValue)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(fieldName));
            }

            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(type));
            }

            if (null == @namespace)
            {
                throw new ArgumentNullException(nameof(@namespace));
            }

            if (null == fieldValue)
            {
                throw new ArgumentNullException(nameof(fieldValue));

            }

            this.Type = new RosType(@namespace, type);
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
