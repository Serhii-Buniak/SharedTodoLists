namespace SharedTodoLists.Application.DTOs.Responses;

public record TodoListSummary
{
    public required string Id { get; init; }
    public required string Name { get; init; }
}
