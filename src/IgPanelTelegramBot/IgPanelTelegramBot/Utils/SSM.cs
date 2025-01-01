using System.Text.Json;
using System.Text.Json.Serialization;
using IgPanelTelegramBot.Models;

namespace IgPanelTelegramBot.Utils;
internal sealed class SSM
{
    private readonly AppSettings _appSettings;

    internal SSM(AppSettings appSettings)
    {
        _appSettings = appSettings;
    }

    internal async Task<UserBalance?> GetUserBalanceAsync()
    {
        using HttpClient httpClient = new();

        Dictionary<string, string> paramsValue = new()
        {
            { "key" , _appSettings.ApiKey },
            { "action" , "balance" }
        };
        var content = new FormUrlEncodedContent(paramsValue);

        var response = await httpClient.PostAsync(_appSettings.BaseApiUrl, content);
        string responseString = await response.Content.ReadAsStringAsync();
        UserBalance? userBalance = JsonSerializer.Deserialize<UserBalance>(responseString);

        return userBalance;
    }

    internal async Task<Service?> GetServiceAsync(string serviceId)
    {
        IEnumerable<Service>? services = await GetAllServicesAsync();

        if (services is null)
        {
            throw new NullReferenceException(nameof(services));
        }

        Service? service = services.SingleOrDefault(x => x.Id == serviceId);

        return service;
    }

    internal async Task<IEnumerable<Service>?> GetAllServicesAsync()
    {
        using HttpClient httpClient = new();

        Dictionary<string, string> paramsValue = new()
        {
            { "key" , _appSettings.ApiKey },
            { "action" , "services" }
        };

        var content = new FormUrlEncodedContent(paramsValue);

        var response = await httpClient.PostAsync(_appSettings.BaseApiUrl, content);
        string responseString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        IEnumerable<Service>? services = JsonSerializer.Deserialize<IEnumerable<Service>>(responseString, options);

        return services;
    }

    internal async Task<DetailsOrder?> CreateOrder(string serviceId, string link, string quantity)
    {
        using HttpClient httpClient = new();

        Dictionary<string, string> paramsValue = new()
        {
            { "key" , _appSettings.ApiKey },
            { "action" , "add" },
            { "service" , serviceId },
            { "link" , link },
            { "quantity" , quantity }
        };

        var content = new FormUrlEncodedContent(paramsValue);

        var response = await httpClient.PostAsync(_appSettings.BaseApiUrl, content);
        string responseString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        DetailsOrder? detailsOrder = JsonSerializer.Deserialize<DetailsOrder>(responseString, options);

        return detailsOrder;
    }

    internal async Task<OrderStatus?> GetStatusOrderAsync(int orderId)
    {
        using HttpClient httpClient = new();

        Dictionary<string, string> paramsValue = new()
        {
            { "key" , _appSettings.ApiKey },
            { "action" , "status" },
            { "order" , orderId.ToString() }
        };

        var content = new FormUrlEncodedContent(paramsValue);

        var response = await httpClient.PostAsync(_appSettings.BaseApiUrl, content);
        string responseString = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        OrderStatus? orderStatus = JsonSerializer.Deserialize<OrderStatus>(responseString, options);

        return orderStatus;
    }
}