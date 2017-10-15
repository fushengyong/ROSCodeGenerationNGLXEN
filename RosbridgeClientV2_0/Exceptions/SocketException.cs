namespace RosbridgeClientV2_0.Exceptions
{
    using System;

    public class SocketException : Exception
    {
        public SocketException() : base() { }
        public SocketException(string message) : base(message) { }
        public SocketException(string message, Exception innerException) : base(message, innerException) { }
    }
}
