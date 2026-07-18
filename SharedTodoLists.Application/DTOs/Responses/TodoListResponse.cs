namespace SharedTodoLists.Application.DTOs.Responses;

public record TodoListResponse(
    string Id,
    string Name,
    string OwnerId,
    DateTime CreatedAt,
    IReadOnlyCollection<string> SharedUserIds);
