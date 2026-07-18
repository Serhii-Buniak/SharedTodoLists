using SharedTodoLists.Application.Models;

namespace SharedTodoLists.Application.Services;

internal interface ITodoListAccessPolicy
{
    bool CanRead(TodoList todoList, string userId);
    bool CanUpdate(TodoList todoList, string userId);
    bool CanDelete(TodoList todoList, string userId);
    bool CanManageUsers(TodoList todoList, string userId);
}
