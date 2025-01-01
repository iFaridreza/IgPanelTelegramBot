using IgPanelTelegramBot.Models;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace IgPanelTelegramBot.Utils;
internal sealed class TelegramBotApi
{
    private readonly TelegramBotClient _telegramBotClient;
    private readonly ILogger _logger;
    private readonly AppSettings _appSettings;
    private readonly TelegramServices _telegramServices;
    private readonly SSM _ssm;

    internal TelegramBotApi(TelegramBotClient telegramBotClient, ILogger logger, AppSettings appSettings, SSM ssm)
    {
        _telegramBotClient = telegramBotClient;
        _logger = logger;
        _appSettings = appSettings;
        _telegramServices = appSettings.TelegramServices;
        _ssm = ssm;
    }

    internal async Task RunAsync()
    {
        _telegramBotClient.OnMessage += _telegramBotClient_OnMessage;
        _telegramBotClient.OnUpdate += _telegramBotClient_OnUpdate;

        User infoBot = await _telegramBotClient.GetMe();
        _logger.Information($"@{infoBot.Username} Run :D");
    }

    private async Task _telegramBotClient_OnUpdate(Update update)
    {
        try
        {
            if (update is null || update.CallbackQuery is null)
            {
                return;
            }

            long userId = update.CallbackQuery.From.Id;

            if (!IsSudo(userId))
            {
                return;
            }

            string? data = update.CallbackQuery.Data;

            if (string.IsNullOrEmpty(data) || update.CallbackQuery.Message is null)
            {
                return;
            }

            int messageId = update.CallbackQuery.Message.MessageId;

            if (data.Equals("alert"))
            {
                string callBackQueryId = update.CallbackQuery.Id;
                await _telegramBotClient.AnswerCallbackQuery(callBackQueryId, Text._alert, showAlert: true);
            }
            else if (data.Equals("back"))
            {
                await _telegramBotClient.DeleteMessage(userId, messageId);

                await _telegramBotClient.SendMessage(userId, Text._backHome, replyMarkup: Keyboard.Home());

                await StepClientManager.Remove(userId);
                UserSession.Remove(userId);
            }
            else if (data.Contains("status_order_"))
            {
                int serviceId = Convert.ToInt32(data.Replace("status_order_", string.Empty).Trim());
                OrderStatus? statusOrder = await _ssm.GetStatusOrderAsync(serviceId);

                if (statusOrder is null)
                {
                    return;
                }

                if (statusOrder.Status == "Completed")
                {
                    await _telegramBotClient.DeleteMessage(userId, messageId);
                    await _telegramBotClient.SendMessage(userId, Text._orderStatusSucsessfully, replyMarkup: Keyboard.Home());
                    return;
                }

                string callBackQueryId = update.CallbackQuery.Id;
                await _telegramBotClient.AnswerCallbackQuery(callBackQueryId, Text._orderStatusPanding, showAlert: true);

            }
            else if (data.Contains("telegram_view_"))
            {
                string serviceId = data.Replace("telegram_view_", string.Empty).Trim();

                ServiceDetails? servicesInfo = _appSettings.TelegramServices.Views.FirstOrDefault(x => x.Id == serviceId);

                if (servicesInfo is null)
                {
                    return;
                }

                Service? serviceDetails = await _ssm.GetServiceAsync(serviceId);

                if (serviceDetails is null)
                {
                    return;
                }

                await StepClientManager.UpdateOrCreate(userId, "get_link");

                UserSession userSession = new(servicesInfo.Id);

                UserSession.UpdateOrCreate(userId, userSession);

                await _telegramBotClient.DeleteMessage(userId, messageId);

                await _telegramBotClient.SendMessage(userId, string.Format(Text._servicesDetails, servicesInfo.Title, serviceDetails.Price, serviceDetails.MinOrder, serviceDetails.MaxOrder), replyMarkup: Keyboard.BackHome());
            }
            else if (data.Contains("telegram_member_"))
            {
                string serviceId = data.Replace("telegram_member_", string.Empty).Trim();

                ServiceDetails? servicesInfo = _appSettings.TelegramServices.Members.FirstOrDefault(x => x.Id == serviceId);

                if (servicesInfo is null)
                {
                    return;
                }

                Service? serviceDetails = await _ssm.GetServiceAsync(serviceId);

                if (serviceDetails is null)
                {
                    return;
                }

                await StepClientManager.UpdateOrCreate(userId, "get_link");

                UserSession userSession = new(servicesInfo.Id);

                UserSession.UpdateOrCreate(userId, userSession);

                await _telegramBotClient.DeleteMessage(userId, messageId);

                await _telegramBotClient.SendMessage(userId, string.Format(Text._servicesDetails, servicesInfo.Title, serviceDetails.Price, serviceDetails.MinOrder, serviceDetails.MaxOrder), replyMarkup: Keyboard.BackHome());
            }
            else if (data.Contains("telegram_reaction_"))
            {
                string serviceId = data.Replace("telegram_reaction_", string.Empty).Trim();

                ServiceDetails? servicesInfo = _appSettings.TelegramServices.Reactions.FirstOrDefault(x => x.Id == serviceId);

                if (servicesInfo is null)
                {
                    return;
                }

                Service? serviceDetails = await _ssm.GetServiceAsync(serviceId);

                if (serviceDetails is null)
                {
                    return;
                }

                await StepClientManager.UpdateOrCreate(userId, "get_link");

                UserSession userSession = new(servicesInfo.Id);

                UserSession.UpdateOrCreate(userId, userSession);

                await _telegramBotClient.DeleteMessage(userId, messageId);

                await _telegramBotClient.SendMessage(userId, string.Format(Text._servicesDetails, servicesInfo.Title, serviceDetails.Price, serviceDetails.MinOrder, serviceDetails.MaxOrder), replyMarkup: Keyboard.BackHome());
            }
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error");
        }
    }

    private async Task _telegramBotClient_OnMessage(Message message, UpdateType type)
    {
        try
        {
            long userId = message.Chat.Id;

            if (message is null || string.IsNullOrEmpty(message.Text) || !IsSudo(userId))
            {
                return;
            }


            switch (message.Text)
            {
                case "/start":
                    {
                        await _telegramBotClient.SendMessage(userId, Text._start, replyMarkup: Keyboard.Home());
                    }
                    break;
                case "ℹ️ اطلاعات حساب":
                    {
                        UserBalance? userBalance = await _ssm.GetUserBalanceAsync();
                        if (userBalance is null)
                        {
                            throw new NullReferenceException(nameof(userBalance));
                        }
                        await _telegramBotClient.SendMessage(userId, string.Format(Text._info, userBalance.Balance), replyMarkup: Keyboard.Home());
                    }
                    break;
                case "خدمات تلگرام🎈":
                    {
                        await _telegramBotClient.SendMessage(userId, Text._telegramServices, replyMarkup: Keyboard.TelegramServices());
                    }
                    break;
                case "خدمات اینستاگرام📍":
                    {
                        await _telegramBotClient.SendMessage(userId, "این بخش در دست بروزرسانی - ریلود میباشد❤️", replyMarkup: Keyboard.Home());
                    }
                    break;
                case "ویو👁‍🗨":
                    {
                        var services = await _ssm.GetAllServicesAsync();
                        await _telegramBotClient.SendMessage(userId, string.Format(Text._telegramServicesDetails, "ویو"), replyMarkup: Keyboard.TelegramViewServices(_telegramServices, services ?? throw new NullReferenceException(nameof(services))));
                    }
                    break;
                case "ممبر👥":
                    {
                        var services = await _ssm.GetAllServicesAsync();
                        await _telegramBotClient.SendMessage(userId, string.Format(Text._telegramServicesDetails, "ممبر"), replyMarkup: Keyboard.TelegramMamberServices(_telegramServices, services ?? throw new NullReferenceException(nameof(services))));
                    }
                    break;
                case "ری اکشن👍":
                    {
                        var services = await _ssm.GetAllServicesAsync();
                        await _telegramBotClient.SendMessage(userId, string.Format(Text._telegramServicesDetails, "ری اکشن"), replyMarkup: Keyboard.TelegramReactionServices(_telegramServices, services ?? throw new NullReferenceException(nameof(services))));
                    }
                    break;
                case "بازگشت⬅️":
                    {
                        await _telegramBotClient.SendMessage(userId, Text._backHome, replyMarkup: Keyboard.Home());


                        await StepClientManager.Remove(userId);
                        UserSession.Remove(userId);
                    }
                    break;
                default:
                    {
                        StepClient? stepClient = await StepClientManager.Get(userId);
                        UserSession? userSession = UserSession.Get(userId);

                        if (stepClient is null || userSession is null)
                        {
                            return;
                        }

                        switch (stepClient.Step)
                        {
                            case "get_link":
                                {
                                    userSession.Link = message.Text;
                                    UserSession.UpdateOrCreate(userId, userSession);
                                    await StepClientManager.UpdateOrCreate(userId, "get_count");

                                    await _telegramBotClient.SendMessage(userId, Text._count, replyMarkup: Keyboard.BackHome());
                                }
                                break;
                            case "get_count":
                                {
                                    if (!long.TryParse(message.Text, out _))
                                    {
                                        await _telegramBotClient.SendMessage(userId, Text._invalidInput, replyMarkup: Keyboard.BackHome());
                                        return;
                                    }

                                    Message msgWite = await _telegramBotClient.SendMessage(userId, Text._wite, replyMarkup: new ReplyKeyboardRemove());

                                    DetailsOrder? detailsOrder = await _ssm.CreateOrder(userSession.ServiceId, userSession.Link, message.Text);

                                    await _telegramBotClient.DeleteMessage(userId, msgWite.MessageId);

                                    await StepClientManager.Remove(userId);
                                    UserSession.Remove(userId);

                                    if (detailsOrder is null || detailsOrder.Status != "success")
                                    {
                                        _logger.Warning($"Error To Create Order {detailsOrder?.Status}");

                                        await _telegramBotClient.SendMessage(userId, Text._errorCreateOrderTry, replyMarkup: Keyboard.Home());
                                        return;
                                    }

                                    await _telegramBotClient.SendMessage(userId, Text._sucsessOrder, replyMarkup: Keyboard.StatusOrder(detailsOrder.OrderId));
                                    await _telegramBotClient.SendMessage(userId, Text._heart, replyMarkup: Keyboard.Home());

                                }
                                break;
                            default: break;
                        }
                    }
                    break;
            }

        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error");
        }
    }

    private bool IsSudo(long userId)
    {
        long[] sudos = _appSettings.Sudos;

        return sudos.Any(x => x == userId);
    }
}