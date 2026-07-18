namespace SharedTodoLists.Application.DTOs.Requests;

public record UpdateTodoListRequest
{
    public required string Name { get; init; }

    public required IReadOnlyList<TodoItemRequest> Items { get; init; }
}

public record TodoItemRequest
{
    public required string Name { get; init; }
    public required bool IsDone { get; init; }
}
