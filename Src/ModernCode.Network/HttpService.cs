using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using ModernCode.Network.Diagnostics;
using ModernCode.Network.Exceptions;
using ModernCode.Utils.Extensions;
using Newtonsoft.Json;

namespace ModernCode.Network
{
    public class HttpService : IDisposable
    {
        private readonly Configuration _configuration;

        public HttpService() : this(null, new HttpClient())
        {
        }

        public HttpService(Configuration configuration, HttpClient httpClient)
        {
            HttpClient = httpClient;
            _configuration = configuration;
        }

        public HttpClient HttpClient { get; }


        public bool IsDiagnosticsEnabled => _configuration?.Diagnostic?.IsEnabled ?? false;

        public void Dispose()
        {
            HttpClient.Dispose();
        }

        private bool IsInternetAvailable()
        {
            return _configuration?.IsInternetFunc == null || _configuration.IsInternetFunc();
        }

        private async Task<HttpResponseMessage> SendInternalAsync(HttpMethod httpMethod, string url, HttpContent content,
            CancellationToken token)
        {
            var request = new HttpRequestMessage(httpMethod, url) {Content = content};
            _configuration?.Diagnostic?.Log(LogLevel.Info, $"Sending {httpMethod} request to {url} {(content != null ? "\n" + content : string.Empty)}", null);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            HttpResponseMessage response;
            try
            {
                response = await HttpClient.SendAsync(request, token).ConfigureAwait(false);
            }
            catch (TaskCanceledException e) when (!token.IsCancellationRequested)
            {
                stopwatch.Stop();
                _configuration?.Diagnostic?.Log(LogLevel.Error, $"Timeout exception in {stopwatch.ElapsedMilliseconds}ms while sending request to {url}", e);
                if (!IsInternetAvailable())
                {
                    throw new NoInternetException(e);
                }
                throw new NetworkException(e);
            }
            catch (HttpRequestException e)
            {
                stopwatch.Stop();
                _configuration?.Diagnostic?.Log(LogLevel.Error, $"Caught exception in {stopwatch.ElapsedMilliseconds}ms while sending request to {url}", e);
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
                _configuration?.Diagnostic?.Log(LogLevel.Error,
                    $"Server responded with error code in {stopwatch.ElapsedMilliseconds}ms {url} {response.StatusCode}:\n{responeText}", null);
                throw new ServerException(response.StatusCode, responeText);
            }
            _configuration?.Diagnostic?.Log(LogLevel.Info, $"Got response from {url} in {stopwatch.ElapsedMilliseconds}ms with status code: {response.StatusCode}", null);
            return response;
        }

        public async Task<HttpResponseMessage> SendAsync(HttpMethod httpMethod, string url, HttpContent content, CancellationToken token)
        {
            return await SendInternalAsync(httpMethod, url, content, token).ConfigureAwait(false);
        }

        public async Task<T> SendAsync<T>(HttpMethod httpMethod, string url, HttpContent content, CancellationToken token)
        {
            var response = await SendAsync(httpMethod, url, content, token).ConfigureAwait(false);
            var responseText = await response.Content.ReadAsStringAsync().WithCancellation(token).ConfigureAwait(false);
            try
            {
                return JsonConvert.DeserializeObject<T>(responseText);
            }
            catch (JsonException e)
            {
                _configuration?.Diagnostic?.Log(LogLevel.Error, $"Failed to deserialize response {responseText}", e);
                throw new ServerException(response.StatusCode, responseText);
            }
        }
    }
}