using Newtonsoft.Json;

namespace ModernCode.Network.Sample.Entities
{
    public class ErrorResponse
    {
        [JsonProperty("error")]
        public string Error { get; set; }
    }
}