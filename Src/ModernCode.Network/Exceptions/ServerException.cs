using System.Net;

namespace ModernCode.Network.Exceptions
{
    public class ServerException : ApiException
    {
        public ServerException(HttpStatusCode statusCode, string response) : base($"Server responded with code: {statusCode}. Response: {response}")
        {
            StatusCode = statusCode;
            Response = response;
        }

        public HttpStatusCode StatusCode { get; set; }
        public string Response { get; set; }
    }
}