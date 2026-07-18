namespace SharedTodoLists.Application.DTOs.Requests;

public record AddTodoListUserRequest
{
    public required string UserId { get; init; }
}
