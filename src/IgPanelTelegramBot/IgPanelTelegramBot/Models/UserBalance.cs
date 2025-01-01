using System.Text.Json.Serialization;

namespace IgPanelTelegramBot.Models;
internal class UserBalance
{
    [JsonPropertyName("balance")]
    public string Balance { get; set; } = string.Empty;
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = string.Empty;
}