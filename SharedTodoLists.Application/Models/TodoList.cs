namespace SharedTodoLists.Application.Models;

public class TodoList
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string OwnerId { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
    public required HashSet<string> SharedUserIds { get; init; }
}
