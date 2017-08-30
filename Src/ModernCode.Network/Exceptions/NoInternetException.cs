using System.Net.Http;

namespace ModernCode.Network.Exceptions
{
    public class NoInternetException : NetworkException
    {
        public NoInternetException() : base("No internet available")
        {
        }

        public NoInternetException(HttpRequestException inner) : base("No internet available", inner)
        {
        }
    }
}