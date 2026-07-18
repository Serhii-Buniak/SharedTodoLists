using SharedTodoLists.Application.Models;

namespace SharedTodoLists.Application.Policies;

internal interface ITodoListAccessPolicy
{
    bool CanAccess(TodoList todoList, string userId);
    bool IsOwner(TodoList todoList, string userId);
}
