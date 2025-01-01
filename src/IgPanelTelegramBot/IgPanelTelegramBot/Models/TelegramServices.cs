using System.Text.Json.Serialization;

namespace IgPanelTelegramBot.Models;

internal sealed class TelegramServices
{
    [JsonPropertyName("Views")]
    public ICollection<ServiceDetails> Views { get; set; } = [];

    [JsonPropertyName("Reactions")]
    public ICollection<ServiceDetails> Reactions { get; set; } = [];

    [JsonPropertyName("Members")]
    public ICollection<ServiceDetails> Members { get; set; } = [];
}