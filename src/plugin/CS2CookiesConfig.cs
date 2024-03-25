using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using CounterStrikeSharp.API.Core;

namespace plugin;

public class CS2CookiesConfig : BasePluginConfig
{
    [JsonPropertyName("DBConnectionString")] public string? DBConnectionString { get; set; }
}