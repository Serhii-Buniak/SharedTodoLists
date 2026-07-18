namespace SharedTodoLists.Application.DTOs.Responses;

public record BatchResponse<T>
{
    public required IReadOnlyList<T> Items { get; init; }
    public required long Total { get; init; }
}
