namespace SharedTodoLists.Persistence.Entities;

public record TodoItemEntry
{
    public required string Name { get; init; }
    public required bool IsDone { get; init; }
}
