using SharedTodoLists.Application.Validation;

namespace SharedTodoLists.Application.DTOs.Requests;

public record CreateTodoListRequest
{
    [TodoListName]
    [NotWhiteSpace]
    public required string Name { get; init; }
}
