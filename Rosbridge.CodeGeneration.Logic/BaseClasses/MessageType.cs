namespace Rosbridge.CodeGeneration.Logic.BaseClasses
{
    using System;
    using System.Collections.Generic;

    public class MessageType
    {
        public string Namespace { get; set; }
        public string Type { get; set; }

        public MessageType(string @namespace, string type)
        {
            if (null == @namespace)
            {
                throw new ArgumentNullException(nameof(@namespace));
            }

            if (null == type)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (string.Empty == type)
            {
                throw new ArgumentException("Parameter cannot be empty!", nameof(type));
            }

            this.Namespace = @namespace;
            this.Type = type;
        }

        public override bool Equals(object obj)
        {
            if (obj is MessageType)
            {
                MessageType other = obj as MessageType;
                return this.Namespace == other.Namespace && this.Type == other.Type;
            }
            return false;
        }

        public override int GetHashCode()
        {
            var hashCode = 158155689;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Namespace);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);
            return hashCode;
        }
    }
}
