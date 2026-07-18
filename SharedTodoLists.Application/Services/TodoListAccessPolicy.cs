using SharedTodoLists.Application.Models;

namespace SharedTodoLists.Application.Policies;

internal class TodoListAccessPolicy : ITodoListAccessPolicy
{
    public bool CanAccess(TodoList todoList, string userId) =>
        IsOwner(todoList, userId) || todoList.SharedUserIds.Contains(userId);

    public bool IsOwner(TodoList todoList, string userId) =>
        todoList.OwnerId == userId;
}
