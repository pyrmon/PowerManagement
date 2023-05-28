using System.Text.Json.Serialization;

namespace PowerManagement.Lambda.Core.Models;

public class RequestModel
{
    [JsonPropertyName("request")] public string? Request { get; set; }
}