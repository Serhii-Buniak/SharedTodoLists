namespace SharedTodoLists.Application.DTOs.Responses;

public record CursorResponse<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public string? NextCursor { get; init; }
    public required bool HasMore { get; init; }
}
