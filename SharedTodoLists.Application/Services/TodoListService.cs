using SharedTodoLists.Application.Abstractions;
using SharedTodoLists.Application.DTOs.Requests;
using SharedTodoLists.Application.DTOs.Responses;
using SharedTodoLists.Application.Exceptions;
using SharedTodoLists.Application.Models;
using SharedTodoLists.Application.Policies;

namespace SharedTodoLists.Application.Services;

internal class TodoListService(
    ITodoListRepository todoListRepository,
    ICurrentUserProvider currentUserProvider,
    ITodoListAccessPolicy accessPolicy) : ITodoListService
{
    public async Task<TodoListResponse> GetTodoListAsync(string id, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserProvider.GetUserId();

        var todoList = await todoListRepository.GetByIdAsync(id, cancellationToken);

        if (todoList is null)
        {
            throw new NotFoundException($"Todo list '{id}' not found.");
        }

        if (!accessPolicy.CanAccess(todoList, currentUserId))
        {
            throw new ForbiddenException("You do not have access to this todo list.");
        }

        return ToResponse(todoList);
    }

    public async Task<TodoListResponse> CreateTodoListAsync(CreateTodoListRequest request, CancellationToken cancellationToken = default)
    {
        var currentUserId = currentUserProvider.GetUserId();
        var now = DateTime.UtcNow;

        var todoList = new TodoList
        {
            Id = string.Empty,
            Name = request.Name,
            OwnerId = currentUserId,
            CreatedAt = now,
            UpdatedAt = now,
            SharedUserIds = []
        };

        var created = await todoListRepository.CreateAsync(todoList, cancellationToken);

        return ToResponse(created);
    }

    private static TodoListResponse ToResponse(TodoList todoList) =>
        new(todoList.Id, todoList.Name, todoList.OwnerId, todoList.CreatedAt, todoList.SharedUserIds);
}
