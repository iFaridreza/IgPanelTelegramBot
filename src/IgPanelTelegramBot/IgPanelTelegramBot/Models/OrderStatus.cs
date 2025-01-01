using System.Text.Json.Serialization;

namespace IgPanelTelegramBot.Models;

internal class OrderStatus
{
    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}
