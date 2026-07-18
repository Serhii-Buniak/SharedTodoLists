using SharedTodoLists.Application.Models;

namespace SharedTodoLists.Application.Abstractions;

public interface ITodoListRepository
{
    Task<TodoList?> GetByIdAsync(string id, CancellationToken cancellationToken = default);
    Task<TodoList> CreateAsync(TodoList todoList, CancellationToken cancellationToken = default);
    Task<TodoList> UpdateAsync(TodoList todoList, CancellationToken cancellationToken = default);
    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
