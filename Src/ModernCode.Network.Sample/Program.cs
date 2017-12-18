using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ModernCode.Network.QueryString;
using ModernCode.Network.Sample.Entities;
using Newtonsoft.Json;

namespace ModernCode.Network.Sample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Request().Wait();
        }

        public static async Task Request()
        {
            var httpService = new HttpService();
            var request = new WeatherRequest {City = "Minsk", AppId = "b6907d289e10d714a6e88b30761fae22"};
            var query = QueryHelper.QueryString(request);
            var response = await httpService.SendAsync<WeatherResponse>(HttpMethod.Get, "http://samples.openweathermap.org/data/2.5/weather?" + query, null,
                CancellationToken.None);

            // Serialize it back for output
            Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        }
    }
}