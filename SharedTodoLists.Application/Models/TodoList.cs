namespace SharedTodoLists.Application.Models;

public record TodoList
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string OwnerId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required IReadOnlySet<string> SharedUserIds { get; init; }
    public required IReadOnlyList<TodoItem> Items { get; init; }
}
