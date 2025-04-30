using System.Text.Json;
using Blazor.models;

namespace Blazor.services;

public interface ICardsService
{
    Task<List<Card>> GetAllCards();
}

public class CardsService : ICardsService
{
    HttpClient _client;
    HttpInfoService _info;

    string AUTH_TOKEN_NAME;
    string CSRF_COOKIE_NAME;


    public CardsService(HttpInfoService httpInfo, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        AUTH_TOKEN_NAME = configuration["AUTH_TOKEN_NAME"] ?? throw new Exception("Missing configuration: AUTH_TOKEN_NAME");
        CSRF_COOKIE_NAME = configuration["CSRF_COOKIE_NAME"] ?? throw new Exception("Missing configuration: CSRF_COOKIE_NAME");
        _info = httpInfo;
        _client = httpClientFactory.CreateClient("main_api");
        _client.DefaultRequestHeaders.Add("Cookie", $"{AUTH_TOKEN_NAME}={_info.ACCESS_TOKEN}");
        _client.DefaultRequestHeaders.Add("Cookie", $"{CSRF_COOKIE_NAME}={_info.CSRF_TOKEN}");
    }

    public async Task<List<Card>> GetAllCards()
    {
        try
        {
            var response = await _client.GetAsync("cards");
            if (!response.IsSuccessStatusCode)
            {
                return [];
            }
            string json = await response.Content.ReadAsStringAsync();

            JsonSerializerOptions JsonOptions = new()
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<Card>>(json, JsonOptions) ?? [];
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return [];
        }
    }
}
