using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ModernCode.Network.Exceptions;
using Newtonsoft.Json;

namespace ModernCode.Network
{
    public class ApiServiceBase : IDisposable
    {
        private readonly Configuration _configuration;
        protected readonly HttpClient HttpClient;

        public ApiServiceBase() : this(null)
        {
        }

        public ApiServiceBase(Configuration configuration)
        {
            HttpClient = new HttpClient();
            _configuration = configuration;
        }

        public void Dispose()
        {
            HttpClient.Dispose();
        }

        private static Uri Endpoint(params string[] args)
        {
            var sb = new StringBuilder();
            foreach (var arg in args)
            {
                sb.Append(arg);
                sb.Append('/');
            }
            sb.Remove(sb.Length - 1, 1);
            return new Uri(sb.ToString());
        }

        private bool IsInternetAvailable()
        {
            return _configuration?.IsInternetFunc == null || _configuration.IsInternetFunc();
        }

        private async Task<HttpResponseMessage> SendInternalAsync(HttpMethod httpMethod, Uri uri, HttpContent content)
        {
            var request = new HttpRequestMessage(httpMethod, uri);
            if (content != null)
            {
                request.Content = content;
            }
            _configuration?.Logger?.Info($"Sending {httpMethod} request to {uri} {(content != null ? "\n" + content : "")}");
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            HttpResponseMessage response;
            try
            {
                response = await HttpClient.SendAsync(request).ConfigureAwait(false);
            }
            catch (HttpRequestException e)
            {
                stopwatch.Stop();
                _configuration?.Logger?.Error($"Caught exception in {stopwatch.ElapsedMilliseconds}ms while sending request to {uri}", e);
                if (!IsInternetAvailable())
                {
                    throw new NoInternetException(e);
                }
                throw new NetworkException(e);
            }

            stopwatch.Stop();

            var success = response.IsSuccessStatusCode;
            if (!success)
            {
                var responeText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                _configuration?.Logger?.Error($"Server responded with error code in {stopwatch.ElapsedMilliseconds}ms {uri} {response.StatusCode}:\n{responeText}");
                throw new ServerException((int) response.StatusCode, responeText);
            }
            _configuration?.Logger?.Info($"Got response from {uri} in {stopwatch.ElapsedMilliseconds}ms with status code: {response.StatusCode}");
            return response;
        }

        protected async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string[] urlParts, string queryString = null, HttpContent content = null)
        {
            if (!IsInternetAvailable())
            {
                throw new NoInternetException();
            }
            var endpoint = Endpoint(urlParts);
            if (!string.IsNullOrWhiteSpace(queryString))
            {
                endpoint = new Uri(endpoint.AbsoluteUri + "?" + queryString);
            }
            return await SendInternalAsync(httpMethod, endpoint, null).ConfigureAwait(false);
        }


        protected async Task<T> SendAsync<T>(HttpMethod httpMethod, string[] urlParts, string queryString = null, HttpContent content = null)
        {
            var response = await SendAsync(httpMethod, urlParts, queryString, content).ConfigureAwait(false);
            var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            try
            {
                return JsonConvert.DeserializeObject<T>(responseText);
            }
            catch (JsonException e)
            {
                _configuration?.Logger?.Error($"Failed to deserialize response {responseText}", e);
                throw new ServerException((int) response.StatusCode, responseText);
            }
        }
    }
}