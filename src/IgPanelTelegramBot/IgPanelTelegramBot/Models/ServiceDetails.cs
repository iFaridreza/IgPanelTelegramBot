using System.Text.Json.Serialization;

namespace IgPanelTelegramBot.Models;
internal sealed class ServiceDetails
{
    [JsonPropertyName("Id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("Title")]
    public string Title { get; set; } = string.Empty;
}
