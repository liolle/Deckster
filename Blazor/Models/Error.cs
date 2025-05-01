using System.Text.Json.Serialization;

namespace Blazor.models;

public class APIError
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = "";
    [JsonPropertyName("value")]
    public string Value { get; set; } = "";

    public override string ToString()
    {
        return $"Error: {Type}:\n - {Value}";
    }
}