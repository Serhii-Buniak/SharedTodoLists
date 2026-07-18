namespace SharedTodoLists.Application.DTOs.Requests;

public record CreateTodoListRequest
{
    public required string Name { get; init; }
}
