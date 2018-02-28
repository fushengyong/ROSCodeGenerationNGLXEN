namespace Rosbridge.CodeGeneration.Logic.BaseClasses
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// ROS message type class. Contains type's namespace and type's name
    /// </summary>
    public class MessageType
    {
        /// <summary>
        /// Type's namespace
        /// </summary>
        public string Namespace { get; private set; }
        /// <summary>
        /// Type's name
        /// </summary>
        public string Type { get; set; }

        public MessageType(string @namespace, string type)
        {
            if (null == @namespace)
            {
                throw new ArgumentNullException(nameof(@namespace));
            }

            if (string.IsNullOrWhiteSpace(type))
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
