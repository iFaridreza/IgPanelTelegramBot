namespace IgPanelTelegramBot.Models;

internal class StepClient
{
    public long UserId { get; init; }
    public string Step { get; set; } = string.Empty;
    public string ExpierDate { get; init; } = string.Empty;
}