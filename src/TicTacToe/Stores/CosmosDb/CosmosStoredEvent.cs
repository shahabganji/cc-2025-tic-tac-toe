using System.Text.Json.Serialization;
using TicTacToe.Domain.Shared;

namespace TicTacToe.Stores.CosmosDb;

internal sealed record CosmosStoredEvent(Guid StreamId, long Version, DateTimeOffset Timestamp, IEvent Event)
    : StoredEvent(StreamId, Version, Event)
{
    [JsonPropertyName("id")] public string Id => $"{Timestamp.ToUnixTimeMilliseconds().ToString()}-{Version}";
    [JsonPropertyName("_ts")] public string Ts => Timestamp.ToString();
    [JsonPropertyName("pk")] public string Pk => StreamId.ToString();
    [JsonPropertyName("type")] public string Type => Event.GetType().Name;
}
