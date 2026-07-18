using SharedTodoLists.Application.Validation;

namespace SharedTodoLists.Application.DTOs.Requests;

public record UpdateTodoListRequest
{
    [TodoListName]
    [NotWhiteSpace]
    public required string Name { get; init; }

    public required IReadOnlyList<TodoItemRequest> Items { get; init; }
}

public record TodoItemRequest
{
    [NotWhiteSpace]
    public required string Name { get; init; }

    public required bool IsDone { get; init; }
}
