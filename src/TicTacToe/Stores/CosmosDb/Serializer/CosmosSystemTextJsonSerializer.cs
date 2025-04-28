using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Cosmos;

namespace TicTacToe.Stores.CosmosDb.Serializer;

/// <summary>
/// Uses <see cref="Azure.Core.Serialization.JsonObjectSerializer"/> which leverages System.Text.Json providing a simple API to interact with on the Azure SDKs.
/// </summary>
/// <remarks>
/// For item CRUD operations and non-LINQ queries, implementing CosmosSerializer is sufficient. To support LINQ query translations as well, CosmosLinqSerializer must be implemented.
/// Inspired from: https://github.com/Azure/azure-cosmos-dotnet-v3/blob/master/Microsoft.Azure.Cosmos.Samples/Usage/SystemTextJson/CosmosSystemTextJsonSerializer.cs
/// </remarks>
// <SystemTextJsonSerializer>
internal class CosmosSystemTextJsonSerializer : CosmosLinqSerializer
{
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public CosmosSystemTextJsonSerializer(JsonSerializerOptions? jsonSerializerOptions = null)
    {
        _jsonSerializerOptions = jsonSerializerOptions ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    public override T FromStream<T>(Stream stream)
    {
        using (stream)
        {
            if (stream is { CanSeek: true, Length: 0 })
            {
                return default!;
            }

            if (typeof(Stream).IsAssignableFrom(typeof(T)))
            {
                return (T)(object)stream;
            }

            return JsonSerializer.Deserialize<T>(stream, options: _jsonSerializerOptions)!;
        }
    }

    public override Stream ToStream<T>(T input)
    {
        var streamPayload = new MemoryStream();
        JsonSerializer.Serialize<T>(streamPayload, input, options: _jsonSerializerOptions);
        streamPayload.Position = 0;

        // var debugPayload = Encoding.UTF8.GetString(streamPayload.ToArray());
        // var debugPayloadString = JsonSerializer.Serialize<T>(input, options: _jsonSerializerOptions);

        return streamPayload;
    }

    public override string SerializeMemberName(MemberInfo memberInfo)
    {
        JsonExtensionDataAttribute? jsonExtensionDataAttribute =
            memberInfo.GetCustomAttribute<JsonExtensionDataAttribute>(true);
        if (jsonExtensionDataAttribute != null)
        {
            return null!;
        }

        JsonPropertyNameAttribute? jsonPropertyNameAttribute =
            memberInfo.GetCustomAttribute<JsonPropertyNameAttribute>(true);
        if (!string.IsNullOrEmpty(jsonPropertyNameAttribute?.Name))
        {
            return jsonPropertyNameAttribute.Name;
        }

        if (_jsonSerializerOptions.PropertyNamingPolicy != null)
        {
            return _jsonSerializerOptions.PropertyNamingPolicy.ConvertName(memberInfo.Name);
        }

        // Do any additional handling of JsonSerializerOptions here.

        return memberInfo.Name;
    }
}
// </SystemTextJsonSerializer>
