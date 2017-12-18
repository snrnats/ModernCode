using ModernCode.Network.QueryString;

namespace ModernCode.Network.Sample.Entities
{
    internal class WeatherRequest
    {
        [QueryParameter("q")]
        public string City { get; set; }

        [QueryParameter("appid")]
        public string AppId { get; set; }
    }
}