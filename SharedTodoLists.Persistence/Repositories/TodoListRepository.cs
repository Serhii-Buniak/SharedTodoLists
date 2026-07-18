using MongoDB.Bson;
using MongoDB.Driver;
using SharedTodoLists.Application.Abstractions;
using SharedTodoLists.Application.DTOs.Responses;
using SharedTodoLists.Application.Models;
using SharedTodoLists.Persistence.Data;
using SharedTodoLists.Persistence.Entities;

namespace SharedTodoLists.Persistence.Repositories;

internal class TodoListRepository(MongoDbContext context) : ITodoListRepository
{
    public async Task<CursorResponse<TodoList>> GetCursorPageAsync(string userId, string? cursor, int limit, bool onlyOwned = false, CancellationToken cancellationToken = default)
    {
        var accessFilter = BuildAccessFilter(userId, onlyOwned);

        var filter = cursor is not null && ObjectId.TryParse(cursor, out var cursorId)
            ? accessFilter & Builders<TodoListEntry>.Filter.Gt(x => x.Id, cursorId)
            : accessFilter;

        var entries = await context.TodoLists
            .Find(filter)
            .Sort(Builders<TodoListEntry>.Sort.Ascending(x => x.Id))
            .Limit(limit + 1)
            .ToListAsync(cancellationToken);

        var hasMore = entries.Count > limit;
        var items = hasMore ? entries.Take(limit).ToList() : entries;

        return new CursorResponse<TodoList>
        {
            Items = items.Select(ToModel).ToList(),
            NextCursor = hasMore ? items.Last().Id.ToString() : null,
            HasMore = hasMore
        };
    }

    public async Task<BatchResponse<TodoList>> GetPageAsync(string userId, int page, int pageSize, bool onlyOwned = false, CancellationToken cancellationToken = default)
    {
        var filter = BuildAccessFilter(userId, onlyOwned);

        var total = await context.TodoLists.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var entries = await context.TodoLists
            .Find(filter)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(cancellationToken);

        return new BatchResponse<TodoList>
        {
            Items = entries.Select(ToModel).ToList(),
            Total = total
        };
    }

    public async Task<TodoList?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        var entry = await context.TodoLists
            .Find(x => x.Id == objectId)
            .FirstOrDefaultAsync(cancellationToken);

        return entry is null ? null : ToModel(entry);
    }

    public async Task<TodoList> CreateAsync(TodoList todoList, CancellationToken cancellationToken = default)
    {
        var entry = ToEntry(todoList, ObjectId.GenerateNewId());
        await context.TodoLists.InsertOneAsync(entry, cancellationToken: cancellationToken);
        return ToModel(entry);
    }

    public async Task<TodoList> UpdateAsync(TodoList todoList, CancellationToken cancellationToken = default)
    {
        var objectId = ObjectId.Parse(todoList.Id);
        var entry = ToEntry(todoList, objectId);
        await context.TodoLists.ReplaceOneAsync(x => x.Id == objectId, entry, cancellationToken: cancellationToken);
        return ToModel(entry);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var objectId = ObjectId.Parse(id);
        await context.TodoLists.DeleteOneAsync(x => x.Id == objectId, cancellationToken);
    }

    private static FilterDefinition<TodoListEntry> BuildAccessFilter(string userId, bool onlyOwned) =>
        onlyOwned
            ? Builders<TodoListEntry>.Filter.Eq(x => x.OwnerId, userId)
            : Builders<TodoListEntry>.Filter.Or(
                Builders<TodoListEntry>.Filter.Eq(x => x.OwnerId, userId),
                Builders<TodoListEntry>.Filter.AnyEq(x => x.SharedUserIds, userId)
            );

    private static TodoListEntry ToEntry(TodoList todoList, ObjectId id) => new()
    {
        Id = id,
        Name = todoList.Name,
        OwnerId = todoList.OwnerId,
        CreatedAt = todoList.CreatedAt,
        UpdatedAt = todoList.UpdatedAt,
        SharedUserIds = new HashSet<string>(todoList.SharedUserIds),
        Items = todoList.Items.Select(i => new TodoItemEntry { Name = i.Name, IsDone = i.IsDone }).ToList()
    };

    private static TodoList ToModel(TodoListEntry entry) => new()
    {
        Id = entry.Id.ToString(),
        Name = entry.Name,
        OwnerId = entry.OwnerId,
        CreatedAt = entry.CreatedAt,
        UpdatedAt = entry.UpdatedAt,
        SharedUserIds = entry.SharedUserIds,
        Items = entry.Items.Select(i => new TodoItem { Name = i.Name, IsDone = i.IsDone }).ToList()
    };
}
