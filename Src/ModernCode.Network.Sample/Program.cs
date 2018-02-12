using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ModernCode.Network.Exceptions;
using ModernCode.Network.Facade;
using ModernCode.Network.QueryString;
using ModernCode.Network.Sample.Entities;
using Newtonsoft.Json;

namespace ModernCode.Network.Sample
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            RequestWithBuilder().Wait();
        }

        public static async Task Request()
        {
            var httpService = new HttpService();
            var request = new WeatherRequest {City = "Minsk", AppId = "b6907d289e10d714a6e88b30761fae22"};
            var query = QueryHelper.QueryString(request);
            var response = await httpService.SendAsync<WeatherResponse>(HttpMethod.Get, "http://samples.openweathermap.org/data/2.5/weather?" + query, null);

            // Serialize it back for console test output
            Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        }

        public static async Task RequestWithBuilder()
        {
            var httpService = new HttpService();
            var request = new WeatherRequest {City = "Minsk", AppId = "b6907d289e10d714a6e88b30761fae22"};
            var query = QueryHelper.QueryString(request);
            var requestBuilder = new Builder<WeatherResponse>(()=>httpService.SendAsync<WeatherResponse>(HttpMethod.Get, "http://samples.openweathermap.org/data/2.5/weather?" + query,
                null));
            requestBuilder.Handle<ErrorResponse>((ex, error) => Console.WriteLine(error.Error));
            requestBuilder.Try<ApiException>(3);
            var response = await requestBuilder.Do();

            // Serialize it back for console test output
            Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        }

        public static async Task RequestWithEx()
        {
            var httpService = new HttpService();
            var request = new WeatherRequest {City = "Minsk", AppId = "b6907d289e10d714a6e88b30761fae22"};
            var query = QueryHelper.QueryString(request);
            var requestBuilder = new Builder<WeatherResponse>(()=>httpService.SendAsync<WeatherResponse>(HttpMethod.Get, "http://samples.openweathermap.org/data/2.5/weather?" + query,
                null));
            requestBuilder.Wrap(func => CommonFacades.Handle<WeatherResponse, ErrorResponse>(func, (ex,error) => Console.WriteLine(error.Error)));
            requestBuilder.Wrap(func => CommonFacades.Retry<WeatherResponse, ApiException>(func, 3));
            var response = await requestBuilder.Do();

            // Serialize it back for console test output
            Console.WriteLine(JsonConvert.SerializeObject(response, Formatting.Indented));
        }
    }
}