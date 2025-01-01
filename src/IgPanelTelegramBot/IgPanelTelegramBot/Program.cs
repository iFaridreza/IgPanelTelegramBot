using IgPanelTelegramBot.Utils;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.TelegramBot;
using Telegram.Bot;

IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").SetBasePath(AppContext.BaseDirectory).Build();

AppSettings appSettings = new();

configuration.Bind(appSettings);

ILogger logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo.TelegramBot(appSettings.Token, appSettings.UsernameLog).CreateLogger();

TelegramBotClient telegramBotClient = new(appSettings.Token);

TelegramBotApi telegramBotApi = new(telegramBotClient, logger, appSettings,new SSM(appSettings));

Thread thread = new(StepClientManager.StartSchadule);

thread.IsBackground = true;

thread.Start();

logger.Information("Step Client Run Backgroud :D");

await telegramBotApi.RunAsync();

Console.ReadKey();