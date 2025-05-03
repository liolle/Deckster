using System.Text;
using System.Text.Json;
using Blazor.models;

namespace Blazor.services;

public interface ICardsService
{
    Task<List<Card>> GetAllCards();
    Task<string> AddCard(AddCardModel card);
    Task<List<Deck>> GetUserDeck();
    Task<string> AddDeck(AddDeckModel deck);
    Task<DeckInfo?> GetDeckInfo(string deckId);
}

public partial class CardsService : ICardsService
{
    HttpClient _client;
    HttpInfoService _info;

    string AUTH_TOKEN_NAME;
    string CSRF_COOKIE_NAME;
    string CSRF_HEADER_NAME;


    public CardsService(HttpInfoService httpInfo, IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        AUTH_TOKEN_NAME = configuration["AUTH_TOKEN_NAME"] ?? throw new Exception("Missing configuration: AUTH_TOKEN_NAME");
        CSRF_COOKIE_NAME = configuration["CSRF_COOKIE_NAME"] ?? throw new Exception("Missing configuration: CSRF_COOKIE_NAME");
        CSRF_HEADER_NAME = configuration["CSRF_HEADER_NAME"] ?? throw new Exception("Missing configuration: CSRF_HEADER_NAME");
        _info = httpInfo;
        _client = httpClientFactory.CreateClient("main_api");

        // AUTH
        _client.DefaultRequestHeaders.Add("Cookie", $"{AUTH_TOKEN_NAME}={_info.ACCESS_TOKEN}");

        // CSRF
        _client.DefaultRequestHeaders.Add("Cookie", $"{CSRF_COOKIE_NAME}={_info.CSRF_COOKIE}");
        _client.DefaultRequestHeaders.Add(CSRF_HEADER_NAME, $"{_info.CSRF_CODE}");
    }
}



// Cards related calls
public partial class CardsService : ICardsService
{
    public async Task<string> AddCard(AddCardModel card)
    {
        try
        {
            string content = JsonSerializer.Serialize(card);
            StringContent httpContent = new(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("card/add", httpContent);


            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                string json = await response.Content.ReadAsStringAsync();

                JsonSerializerOptions JsonOptions = new()
                {
                    PropertyNameCaseInsensitive = true
                };

                APIError? output = JsonSerializer.Deserialize<APIError>(json, JsonOptions);

                return output?.ToString() ?? "";
            }

            response.EnsureSuccessStatusCode();


            return "";
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Error: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return "Failed to Add Card";
    }

    public async Task<List<Card>> GetAllCards()
    {
        try
        {
            HttpResponseMessage response = await _client.GetAsync("cards");
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
        }

        return [];
    }

}


// Deck related calls 
public partial class CardsService
{
    public async Task<List<Deck>> GetUserDeck()
    {
        try
        {
            HttpResponseMessage response = await _client.GetAsync("decks/me");
            if (!response.IsSuccessStatusCode)
            {
                return [];
            }
            string json = await response.Content.ReadAsStringAsync();

            JsonSerializerOptions JsonOptions = new()
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<List<Deck>>(json, JsonOptions) ?? [];
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return [];
    }

    public async Task<string> AddDeck(AddDeckModel deck)
    {
        try
        {
            string content = JsonSerializer.Serialize(deck);
            StringContent httpContent = new(content, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PostAsync("deck/add", httpContent);


            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                string json = await response.Content.ReadAsStringAsync();

                JsonSerializerOptions JsonOptions = new()
                {
                    PropertyNameCaseInsensitive = true
                };


                APIError? output = JsonSerializer.Deserialize<APIError>(json, JsonOptions);

                return output?.ToString() ?? "";
            }


            response.EnsureSuccessStatusCode();


            return "";
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Error: {ex.Message}");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        return "Failed to Add Deck";
    }


    public async Task<DeckInfo?> GetDeckInfo(string deckId)
    {
        try
        {
            HttpResponseMessage response = await _client.GetAsync($"deck/cards?deckId={deckId}");
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            string json = await response.Content.ReadAsStringAsync();


            JsonSerializerOptions JsonOptions = new()
            {
                PropertyNameCaseInsensitive = true
            };

            return JsonSerializer.Deserialize<DeckInfo>(json, JsonOptions) ?? null;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return null;
    }
}