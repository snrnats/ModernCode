namespace ModernCode.Network.Exceptions
{
    public class ServerException : ApiException
    {
        public ServerException(int statusCode, string response) : base($"Server responded with code: {statusCode}. Response: {response}")
        {
            StatusCode = statusCode;
            Response = response;
        }

        public int StatusCode { get; set; }
        public string Response { get; set; }
    }
}