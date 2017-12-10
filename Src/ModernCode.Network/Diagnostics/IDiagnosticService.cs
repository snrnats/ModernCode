using System;

namespace ModernCode.Network.Diagnostics
{
    public interface IDiagnosticService
    {
        bool IsEnabled { get; set; }
        LogLevel MinLevel { get; set; }

        void Log(LogLevel level, string message, Exception e);
    }
}