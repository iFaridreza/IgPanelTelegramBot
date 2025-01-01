using System.Text.Json.Serialization;
using IgPanelTelegramBot.Models;

namespace IgPanelTelegramBot.Utils;
internal sealed class AppSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string BaseApiUrl { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public string UsernameLog { get; set; } = string.Empty;
    public long[] Sudos { get; set; } = [];
    public int TimeOutMinute { get; set; }
    public TelegramServices TelegramServices { get; set; } = new();
    public InstagramServices InstagramServices { get; set; } = new();
}