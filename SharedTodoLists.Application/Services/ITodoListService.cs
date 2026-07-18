using SharedTodoLists.Application.DTOs.Requests;
using SharedTodoLists.Application.DTOs.Responses;

namespace SharedTodoLists.Application.Services;

public interface ITodoListService
{
    Task<PagedResponse<TodoListSummary>> GetTodoListsAsync(int page, int pageSize, bool onlyOwned = false, CancellationToken cancellationToken = default);
    Task<CursorResponse<TodoListResponse>> GetTodoListsStreamAsync(string? cursor, int limit, bool onlyOwned = false, CancellationToken cancellationToken = default);
    Task<TodoListResponse> GetTodoListAsync(string id, CancellationToken cancellationToken = default);
    Task<TodoListResponse> CreateTodoListAsync(CreateTodoListRequest request, CancellationToken cancellationToken = default);
    Task<TodoListResponse> UpdateTodoListAsync(string id, UpdateTodoListRequest request, CancellationToken cancellationToken = default);
    Task<TodoListUsersResponse> GetTodoListUsersAsync(string id, CancellationToken cancellationToken = default);
    Task<TodoListUsersResponse> AddTodoListUserAsync(string id, AddTodoListUserRequest request, CancellationToken cancellationToken = default);
    Task RemoveTodoListUserAsync(string id, string userId, CancellationToken cancellationToken = default);
    Task DeleteTodoListAsync(string id, CancellationToken cancellationToken = default);
}
