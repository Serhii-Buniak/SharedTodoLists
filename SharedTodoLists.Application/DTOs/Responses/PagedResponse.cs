namespace SharedTodoLists.Application.DTOs.Responses;

public record PagedResponse<T> : BatchResponse<T>
{
    public required int Page { get; init; }
    public required int PageSize { get; init; }
}
