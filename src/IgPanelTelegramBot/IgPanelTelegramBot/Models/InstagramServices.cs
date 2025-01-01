using System.Text.Json.Serialization;

namespace IgPanelTelegramBot.Models;

internal sealed class InstagramServices
{
    [JsonPropertyName("ViewsStory")]
    public ICollection<ServiceDetails> ViewsStory { get; set; } = [];

    [JsonPropertyName("ViewsVideo")]
    public ICollection<ServiceDetails> ViewsVideo { get; set; } = [];

    [JsonPropertyName("Likes")]
    public ICollection<ServiceDetails> Likes { get; set; } = []; 
    
    [JsonPropertyName("Followers")]
    public ICollection<ServiceDetails> Followers { get; set; } = [];
}