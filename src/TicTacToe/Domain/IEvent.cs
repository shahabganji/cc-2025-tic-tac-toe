using System.Text.Json.Serialization;
using TicTacToe.Domain.Games.Events;
using TicTacToe.Domain.Games.LoadGamesFeature;
using TicTacToe.Domain.Players.Events;

namespace TicTacToe.Domain;

[JsonPolymorphic(IgnoreUnrecognizedTypeDiscriminators = true)]
[JsonDerivedType(typeof(GameCreated), nameof(GameCreated))]
[JsonDerivedType(typeof(PlayerJoined), nameof(PlayerJoined))]
[JsonDerivedType(typeof(PlayerRegistered), nameof(PlayerRegistered))]
[JsonDerivedType(typeof(CellFilled), nameof(CellFilled))]
[JsonDerivedType(typeof(GameFinished), nameof(GameFinished))]
public interface IEvent;
