using TicTacToe.Domain;
using Microsoft.Azure.Cosmos;
using TicTacToe.Web.Contracts;


namespace TicTacToe.Stores.CosmosDb;

internal sealed partial class CosmosEventStore : IQueryStore
{
    public async Task<IEnumerable<GameInfo>> GetAvailableGames()
    {
        var streamIterator = _container.GetItemQueryIterator<GameInfo>(
            new QueryDefinition(
                "SELECT IS_DEFINED(c.Event.GameId) as IsFinished, IS_DEFINED(c.Event.Id) ? c.Event.Id  : c.Event.GameId as Id, c.Event.Name FROM c WHERE c.type = 'GameCreated' or c.type = 'GameFinished'"));

        if (!streamIterator.HasMoreResults)
            return [];

        var games = new List<GameInfo>();

        while (streamIterator.HasMoreResults)
        {
            var readNext = await streamIterator.ReadNextAsync();

            if (readNext.Count == 0)
                continue;

            games.AddRange(readNext.Resource);
        }

        var finishedGames = games.Where(x => x.IsFinished).Select(x => x.Id);
        var availableGames = games.Where(x => !finishedGames.Contains(x.Id));

        return availableGames;
    }
}
