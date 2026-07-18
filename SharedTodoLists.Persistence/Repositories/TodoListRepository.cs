using MongoDB.Bson;
using MongoDB.Driver;
using SharedTodoLists.Application.Abstractions;
using SharedTodoLists.Application.Models;
using SharedTodoLists.Persistence.Data;
using SharedTodoLists.Persistence.Entities;

namespace SharedTodoLists.Persistence.Repositories;

internal class TodoListRepository(MongoDbContext context) : ITodoListRepository
{
    public async Task<TodoList?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(id, out var objectId))
        {
            return null;
        }

        var entry = await context.TodoLists
            .Find(x => x.Id == objectId)
            .FirstOrDefaultAsync(cancellationToken);

        return entry is null ? null : ToModel(entry);
    }

    public async Task<TodoList> CreateAsync(TodoList todoList, CancellationToken cancellationToken = default)
    {
        var entry = new TodoListEntry
        {
            Id = ObjectId.GenerateNewId(),
            Name = todoList.Name,
            OwnerId = todoList.OwnerId,
            CreatedAt = todoList.CreatedAt,
            UpdatedAt = todoList.UpdatedAt,
            SharedUserIds = todoList.SharedUserIds
        };

        await context.TodoLists.InsertOneAsync(entry, cancellationToken: cancellationToken);

        return ToModel(entry);
    }

    private static TodoList ToModel(TodoListEntry entry) => new()
    {
        Id = entry.Id.ToString(),
        Name = entry.Name,
        OwnerId = entry.OwnerId ?? string.Empty,
        CreatedAt = entry.CreatedAt,
        UpdatedAt = entry.UpdatedAt,
        SharedUserIds = entry.SharedUserIds
    };
}
