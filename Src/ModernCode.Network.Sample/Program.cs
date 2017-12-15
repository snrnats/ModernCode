using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ModernCode.Network.QueryString;
using ModernCode.Network.Sample.Entities;

namespace ModernCode.Network.Sample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var httpService = new HttpService();
            var request = new WeatherRequest(){City = "Minsk", AppId = "b6907d289e10d714a6e88b30761fae22" };
            var query = QueryHelper.QueryString(request);
            var response = await httpService.SendAsync<WeatherResponse>(HttpMethod.Get, "http://samples.openweathermap.org/data/2.5/weather", null,
                CancellationToken.None);

        }
    }
}
