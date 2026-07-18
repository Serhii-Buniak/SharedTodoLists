using SharedTodoLists.Application.Exceptions;
using SharedTodoLists.Application.Models;
using SharedTodoLists.Application.Services;

namespace SharedTodoLists.Application.Extensions;

internal static class TodoListAccessPolicyExtensions
{
    internal static void EnsureCanRead(this ITodoListAccessPolicy policy, TodoList todoList, string userId)
    {
        if (!policy.CanRead(todoList, userId))
            throw new ForbiddenException("You do not have permission to view this todo list.");
    }

    internal static void EnsureCanUpdate(this ITodoListAccessPolicy policy, TodoList todoList, string userId)
    {
        if (!policy.CanUpdate(todoList, userId))
            throw new ForbiddenException("You do not have permission to update this todo list.");
    }

    internal static void EnsureCanDelete(this ITodoListAccessPolicy policy, TodoList todoList, string userId)
    {
        if (!policy.CanDelete(todoList, userId))
            throw new ForbiddenException("You do not have permission to delete this todo list.");
    }

    internal static void EnsureCanManageUsers(this ITodoListAccessPolicy policy, TodoList todoList, string userId)
    {
        if (!policy.CanManageUsers(todoList, userId))
            throw new ForbiddenException("You do not have permission to manage users for this todo list.");
    }
}
