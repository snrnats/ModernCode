using System;

namespace ModernCode.Network.Exceptions
{
    public class NoInternetException : NetworkException
    {
        public NoInternetException() : base("No internet available")
        {
        }

        public NoInternetException(Exception inner) : base("No internet available", inner)
        {
        }
    }
}