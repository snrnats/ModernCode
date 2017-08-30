using System;
using System.Net.Http;

namespace ModernCode.Network
{
    public class Configuration
    {
        public HttpMessageHandler Handler { get; set; }
        public Func<bool> IsInternetFunc { get; set; }
        public ILogger Logger { get; set; }
    }
}