using SharedTodoLists.Application.DTOs.Requests;
using SharedTodoLists.Application.DTOs.Responses;

namespace SharedTodoLists.Application.Services;

public interface ITodoListService
{
    Task<TodoListResponse> GetTodoListAsync(string id, CancellationToken cancellationToken = default);
    Task<TodoListResponse> CreateTodoListAsync(CreateTodoListRequest request, CancellationToken cancellationToken = default);
    Task DeleteTodoListAsync(string id, CancellationToken cancellationToken = default);
}
