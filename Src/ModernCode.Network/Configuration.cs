using System;
using System.Net.Http;
using ModernCode.Network.Diagnostics;

namespace ModernCode.Network
{
    public class Configuration
    {
        public HttpMessageHandler Handler { get; set; }
        public Func<bool> IsInternetFunc { get; set; }
        public IDiagnosticService Diagnostic { get; set; }
        public bool IsDiagnosticsEnabled => Diagnostic?.IsEnabled ?? false;
    }
}