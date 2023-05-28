using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace PowerManagement.Lambda.Core.Models;

[ExcludeFromCodeCoverage]
public class ActionPlan
{
    [JsonProperty("actions")] public List<ActionElement> Actions { get; set; } = null!;
}

[ExcludeFromCodeCoverage]
public class ActionElement
{
    [JsonProperty("action")] public string Action { get; set; } = null!;
    [JsonProperty("command")] public string Command { get; set; } = null!;
    [JsonProperty("hostname")] public string Hostname { get; set; } = null!;
    [JsonProperty("port")] public int Port { get; set; }
    [JsonProperty("message")] public string Message { get; set; } = null!;
    [JsonProperty("username")] public string Username { get; set; } = null!;
    [JsonProperty("ssmKeyPath")] public string SsmKeyPath { get; set; } = null!;
}