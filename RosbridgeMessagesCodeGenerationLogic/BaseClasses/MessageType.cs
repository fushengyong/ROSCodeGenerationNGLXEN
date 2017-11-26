﻿namespace RosbridgeMessagesCodeGenerationLogic.BaseClasses
{
    using System;
    using System.Collections.Generic;

    public class MessageType
    {
        public string NamespaceName { get; set; }
        public string TypeName { get; set; }

        public MessageType(string namespaceName, string typeName)
        {
            if (null == namespaceName)
            {
                throw new ArgumentNullException(nameof(namespaceName));
            }

            if (null == typeName)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            if (string.Empty == typeName)
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(typeName));
            }

            this.NamespaceName = namespaceName;
            this.TypeName = typeName;
        }

        public override bool Equals(object obj)
        {
            if (obj is MessageType)
            {
                MessageType other = obj as MessageType;
                return this.NamespaceName == other.NamespaceName && this.TypeName == other.TypeName;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = 158155689;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(NamespaceName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TypeName);
            return hashCode;
        }
    }
}
