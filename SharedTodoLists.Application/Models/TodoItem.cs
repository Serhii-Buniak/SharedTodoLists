namespace SharedTodoLists.Application.Models;

public record TodoItem
{
    public required string Name { get; init; }
    public required bool IsDone { get; init; }
}
