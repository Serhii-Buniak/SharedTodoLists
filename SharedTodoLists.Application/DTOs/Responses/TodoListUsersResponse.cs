namespace SharedTodoLists.Application.DTOs.Responses;

public record TodoListUsersResponse
{
    public required string OwnerId { get; init; }
    public required IReadOnlySet<string> SharedUserIds { get; init; }
}
