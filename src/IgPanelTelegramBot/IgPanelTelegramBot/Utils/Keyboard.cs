using IgPanelTelegramBot.Models;
using Telegram.Bot.Types.ReplyMarkups;

namespace IgPanelTelegramBot.Utils;
internal static class Keyboard
{
    internal static ReplyKeyboardMarkup Home()
    {
        ReplyKeyboardMarkup replyKeyboardMarkup = new();
        replyKeyboardMarkup.AddButtons(["خدمات تلگرام🎈", "خدمات اینستاگرام📍"]).AddNewRow().AddButton("ℹ️ اطلاعات حساب");
        replyKeyboardMarkup.ResizeKeyboard = true;
        return replyKeyboardMarkup;
    }

    internal static ReplyKeyboardMarkup TelegramServices()
    {
        ReplyKeyboardMarkup replyKeyboardMarkup = new();
        replyKeyboardMarkup.AddButtons(["ممبر👥", "ری اکشن👍"]).AddNewRow().AddButtons(["ویو👁‍🗨", "بازگشت⬅️"]);
        replyKeyboardMarkup.ResizeKeyboard = true;
        return replyKeyboardMarkup;
    }

    internal static InlineKeyboardMarkup BackHomeInline()
    {
        InlineKeyboardMarkup replyKeyboardMarkup = new();

        replyKeyboardMarkup.AddButton("بازگشت⬅️", "back");

        return replyKeyboardMarkup;
    }  
    
    internal static InlineKeyboardMarkup StatusOrder(int servicesId)
    {
        InlineKeyboardMarkup replyKeyboardMarkup = new();

        replyKeyboardMarkup.AddButton("بررسی✨", $"status_order_{servicesId}");

        return replyKeyboardMarkup;
    }

    internal static ReplyKeyboardMarkup BackHome()
    {
        ReplyKeyboardMarkup replyKeyboardMarkup = new();
        replyKeyboardMarkup.AddButton("بازگشت⬅️");
        replyKeyboardMarkup.ResizeKeyboard = true;
        return replyKeyboardMarkup;
    }

    internal static InlineKeyboardMarkup TelegramViewServices(TelegramServices telegramServices, IEnumerable<Service> services)
    {
        InlineKeyboardMarkup replyKeyboardMarkup = new();

        replyKeyboardMarkup.AddButton("💲","alert").AddButton("ℹ️","alert").AddNewRow();

        foreach (var item in telegramServices.Views)
        {
            var service = services.FirstOrDefault(x => x.Id == item.Id);

            if (service is null)
            {
                continue;
            }

            replyKeyboardMarkup.AddButton(service.Price, "alert").AddButton(item.Title, $"telegram_view_{item.Id}").AddNewRow();
        }

        return replyKeyboardMarkup;
    }

    internal static InlineKeyboardMarkup TelegramMamberServices(TelegramServices telegramServices, IEnumerable<Service> services)
    {
        InlineKeyboardMarkup replyKeyboardMarkup = new();

        replyKeyboardMarkup.AddButton("💲", "alert").AddButton("ℹ️", "alert").AddNewRow();

        foreach (var item in telegramServices.Members)
        {
            var service = services.FirstOrDefault(x => x.Id == item.Id);

            if (service is null)
            {
                continue;
            }

            replyKeyboardMarkup.AddButton(service.Price, "alert").AddButton(item.Title, $"telegram_member_{item.Id}").AddNewRow();
        }

        return replyKeyboardMarkup;
    }

    internal static InlineKeyboardMarkup TelegramReactionServices(TelegramServices telegramServices, IEnumerable<Service> services)
    {
        InlineKeyboardMarkup replyKeyboardMarkup = new();

        replyKeyboardMarkup.AddButton("💲", "alert").AddButton("ℹ️", "alert").AddNewRow();

        foreach (var item in telegramServices.Reactions)
        {
            var service = services.FirstOrDefault(x => x.Id == item.Id);

            if (service is null)
            {
                continue;
            }

            replyKeyboardMarkup.AddButton(service.Price, "alert").AddButton(item.Title, $"telegram_reaction_{item.Id}").AddNewRow();
        }

        return replyKeyboardMarkup;
    }
}