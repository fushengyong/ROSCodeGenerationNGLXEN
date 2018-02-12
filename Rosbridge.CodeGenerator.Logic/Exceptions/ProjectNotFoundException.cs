namespace Rosbridge.CodeGenerator.Logic.Exceptions
{
    using System;

    public class ProjectNotFoundException : Exception
    {
        public ProjectNotFoundException() : base() { }
        public ProjectNotFoundException(string message) : base(message) { }
        public ProjectNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
}
