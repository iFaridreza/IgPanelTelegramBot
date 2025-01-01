using System.Text.Json.Serialization;

namespace IgPanelTelegramBot.Models;

internal class DetailsOrder
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("order")]
    public int OrderId { get; set; }
}
