using MongoDB.Driver;
using SharedTodoLists.Persistence.Configs;
using SharedTodoLists.Persistence.Entities;

namespace SharedTodoLists.Persistence.Data;

public sealed class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(
        IMongoClient client,
        MongoDbOptions options)
    {
        _database = client.GetDatabase(options.DatabaseName);
    }

    public IMongoCollection<TodoListEntry> TodoLists => _database.GetCollection<TodoListEntry>("todoLists");

    public async Task CreateIndexesAsync(CancellationToken cancellationToken)
    {
        var models = new[]
        {
            new CreateIndexModel<TodoListEntry>(
                Builders<TodoListEntry>.IndexKeys
                    .Ascending(x => x.OwnerId)),

            new CreateIndexModel<TodoListEntry>(
                Builders<TodoListEntry>.IndexKeys
                    .Ascending(x => x.SharedUserIds)),

            new CreateIndexModel<TodoListEntry>(
                Builders<TodoListEntry>.IndexKeys
                    .Descending(x => x.CreatedAt))
        };

        await TodoLists.Indexes.CreateManyAsync(models, cancellationToken);
    }
}