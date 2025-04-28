using System.Text.Json.Serialization;

namespace TicTacToe.Stores.CosmosDb;

internal sealed record CosmosEventStream(string StreamId)
{
    internal static string GetId(string streamId) => $"EventStream-{streamId}";

    public long Version { get; set; }

    [JsonPropertyName("id")] public string Id => GetId(StreamId);

    [JsonPropertyName("_etag")] public string Etag { get; set; }
};