namespace Rosbridge.CodeGeneration.Logic.Exceptions
{
    using System;

    public class NoStandardNamespaceException : Exception
    {
        public NoStandardNamespaceException() : base() { }
        public NoStandardNamespaceException(string message) : base(message) { }
        public NoStandardNamespaceException(string message, Exception innerException) : base(message, innerException) { }
    }
}
