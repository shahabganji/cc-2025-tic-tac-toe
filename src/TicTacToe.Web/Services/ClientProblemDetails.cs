using System.Text.Json.Serialization;

namespace TicTacToe.Web.Services;

public class ClientProblemDetails
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;

    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    [JsonPropertyName("status")]
    public int Status { get; set; } = default!;

    [JsonPropertyName("detail")]
    public string Detail { get; set; } = default!;

    [JsonPropertyName("instance")]
    public string Instance { get; set; } = default!;
}
