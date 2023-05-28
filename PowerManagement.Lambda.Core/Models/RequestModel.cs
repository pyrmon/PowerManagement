using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace PowerManagement.Lambda.Core.Models;

[ExcludeFromCodeCoverage]
public class RequestModel
{
    [JsonProperty("request")] public string? Request { get; set; }
}