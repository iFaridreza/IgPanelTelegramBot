using IgPanelTelegramBot.Models;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using Telegram.Bot;
using TimerSchadule = System.Timers.Timer;

namespace IgPanelTelegramBot.Utils;

internal static class StepClientManager
{
    private const string _filePathStep = "stepclient.json";
    private static SemaphoreSlim semaphoreSlim = new(1);
    private static TimerSchadule _timerSchaduel;
    private static TelegramBotClient _botClient;
    private static AppSettings _appSettings;

    static StepClientManager()
    {
        if (!File.Exists(_filePathStep))
        {
            File.Create(_filePathStep).Close();
        }

        IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").SetBasePath(AppContext.BaseDirectory).Build();

        _appSettings = new();

        configuration.Bind(_appSettings);

        _botClient = new(_appSettings.Token);

        _timerSchaduel = new(TimeSpan.FromMinutes(_appSettings.TimeOutMinute));

        _timerSchaduel.Elapsed += _timerSchaduel_Elapsed;
    }

    internal static void StartSchadule()
    {
        _timerSchaduel.Start();
    }

    private async static void _timerSchaduel_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        string? allText = await GetAllText();

        if (string.IsNullOrEmpty(allText)) return;

        ICollection<StepClient>? stepClients = JsonSerializer.Deserialize<ICollection<StepClient>>(allText);

        if (stepClients is null || stepClients.Count == 0) return;

        DateTime dateTimeNow = DateTime.Now;

        var stepExpierClient = stepClients.Where(x => Convert.ToDateTime(x.ExpierDate) < dateTimeNow).ToList();

        foreach (var item in stepExpierClient)
        {
            await Remove(item.UserId);

            try
            {
                await _botClient.SendMessage(item.UserId, Text._timeOut, replyMarkup: Keyboard.Home());

                UserSession.Remove(item.UserId);
            }
            catch
            {
                continue;
            }
        }
    }

    internal static async Task<ICollection<StepClient>?> GetAll()
    {
        string? allText = await GetAllText();

        if (string.IsNullOrEmpty(allText)) return null;

        ICollection<StepClient>? stepClients = JsonSerializer.Deserialize<ICollection<StepClient>>(allText);

        return stepClients;
    }

    internal static async Task UpdateOrCreate(long userId, string step)
    {
        ICollection<StepClient>? stepClients = await GetAll();

        if (stepClients is null)
        {
            stepClients = new List<StepClient>();
        };

        StepClient? stepClientFirst = stepClients.FirstOrDefault(x => x.UserId == userId);

        string timeExpier = DateTime.Now.AddMinutes(_appSettings.TimeOutMinute).ToString("HH:MM:ss");

        if (stepClientFirst is null)
        {
            stepClients.Add(new()
            {
                UserId = userId,
                Step = step,
                ExpierDate = timeExpier
            });

            await WriteAllText(stepClients);

            return;
        }

        stepClients.Remove(stepClientFirst);

        stepClients.Add(new()
        {
            UserId = userId,
            Step = step,
            ExpierDate = timeExpier
        });

        await WriteAllText(stepClients);
    }

    internal static async Task Remove(long userId)
    {
        ICollection<StepClient>? stepClients = await GetAll();

        if (stepClients is null) return;

        StepClient? stepClientFirst = stepClients.FirstOrDefault(x => x.UserId == userId);

        if (stepClientFirst is null) return;

        stepClients.Remove(stepClientFirst);

        await WriteAllText(stepClients);
    }

    internal static async Task<StepClient?> Get(long userId)
    {
        ICollection<StepClient>? stepClients = await GetAll();

        if (stepClients is null) return null;

        StepClient? stepClientFirst = stepClients.FirstOrDefault(x => x.UserId == userId);

        return stepClientFirst;
    }

    private async static Task<string> GetAllText()
    {
        semaphoreSlim.Wait();
        string? allText = await File.ReadAllTextAsync(_filePathStep);
        semaphoreSlim.Release();
        return allText;
    }

    private async static Task WriteAllText(IEnumerable<StepClient> stepClients)
    {
        semaphoreSlim.Wait();
        string jsonSerilize = JsonSerializer.Serialize(stepClients, new JsonSerializerOptions()
        {
            WriteIndented = true
        });
        await File.WriteAllTextAsync(_filePathStep, jsonSerilize);
        semaphoreSlim.Release();
    }
}
