using SharedTodoLists.Application.Models;

namespace SharedTodoLists.Application.Services;

internal class TodoListAccessPolicy : ITodoListAccessPolicy
{
    public bool CanRead(TodoList todoList, string userId) => CanAccess(todoList, userId);
    public bool CanUpdate(TodoList todoList, string userId) => CanAccess(todoList, userId);
    public bool CanDelete(TodoList todoList, string userId) => IsOwner(todoList, userId);
    public bool CanManageUsers(TodoList todoList, string userId) => CanAccess(todoList, userId);

    private static bool CanAccess(TodoList todoList, string userId) =>
        IsOwner(todoList, userId) || todoList.SharedUserIds.Contains(userId);

    private static bool IsOwner(TodoList todoList, string userId) =>
        todoList.OwnerId == userId;
}
