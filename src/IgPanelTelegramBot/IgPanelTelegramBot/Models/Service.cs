using System.Text.Json.Serialization;

namespace IgPanelTelegramBot.Models;
internal class Service
{
    [JsonPropertyName("service")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("rate")]
    public string Price { get; set; } = string.Empty;
    
    [JsonPropertyName("min")]
    public string MinOrder { get; set; } = string.Empty;

    [JsonPropertyName("max")]
    public string MaxOrder { get; set; } = string.Empty;
}
