using System;

namespace ModernCode.Network.Exceptions
{
    public class NetworkException : ApiException
    {
        public NetworkException(string message) : base(message)
        {
        }

        public NetworkException(Exception inner) : base("Network exception", inner)
        {
        }

        public NetworkException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}