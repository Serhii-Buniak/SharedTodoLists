using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedTodoLists.Persistence.Entities;

public record TodoListEntry
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public ObjectId Id { get; init; }
    public required string OwnerId { get; init; }
    public required string Name { get; init; }
    public required DateTime CreatedAt { get; init; } 
    public required DateTime UpdatedAt { get; set; }
    public required HashSet<string> SharedUserIds { get; init; } = [];
    public required List<TodoItemEntry> Items { get; init; } = [];
}