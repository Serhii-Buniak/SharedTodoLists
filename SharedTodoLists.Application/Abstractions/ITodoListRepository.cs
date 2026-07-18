using SharedTodoLists.Application.DTOs.Responses;
using SharedTodoLists.Application.Models;

namespace SharedTodoLists.Application.Abstractions;

public interface ITodoListRepository
{
    Task<BatchResponse<TodoList>> GetPageAsync(string userId, int page, int pageSize, bool onlyOwned = false, CancellationToken cancellationToken = default);
    Task<CursorResponse<TodoList>> GetCursorPageAsync(string userId, string? cursor, int limit, bool onlyOwned = false, CancellationToken cancellationToken = default);
    Task<TodoList?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<TodoList> CreateAsync(TodoList todoList, CancellationToken cancellationToken = default);
    Task<TodoList> UpdateAsync(TodoList todoList, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
