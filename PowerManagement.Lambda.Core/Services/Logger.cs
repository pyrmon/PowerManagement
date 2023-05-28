using System.Diagnostics.CodeAnalysis;
using Amazon.Lambda.Core;
using PowerManagement.Lambda.Core.Contracts;

namespace PowerManagement.Lambda.Core.Services;

[ExcludeFromCodeCoverage]
public class Logger : ILogger
{
    public void LogInfo(string message)
    {
        LambdaLogger.Log($"[INFO] {message}");
    }

    public void LogWarning(string message)
    {
        LambdaLogger.Log($"[WARN] {message}");
    }

    public void LogError(string message)
    {
        LambdaLogger.Log($"[ERROR] {message}");
    }
}