using SharedTodoLists.Application.Abstractions;
using SharedTodoLists.Application.DTOs.Requests;
using SharedTodoLists.Application.DTOs.Responses;
using SharedTodoLists.Application.Exceptions;
using SharedTodoLists.Application.Models;
using SharedTodoLists.Application.Policies;

namespace SharedTodoLists.Application.Services;

internal class TodoListService : ITodoListService
{
    private readonly ITodoListRepository _todoListRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ITodoListAccessPolicy _accessPolicy;

    public TodoListService(ITodoListRepository todoListRepository,
        ICurrentUserProvider currentUserProvider,
        ITodoListAccessPolicy accessPolicy)
    {
        _todoListRepository = todoListRepository;
        _currentUserProvider = currentUserProvider;
        _accessPolicy = accessPolicy;
    }

    public async Task<TodoListResponse> GetTodoListAsync(string id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserProvider.GetUserId();

        var todoList = await _todoListRepository.GetByIdAsync(id, cancellationToken);

        if (todoList is null)
        {
            throw new NotFoundException($"Todo list '{id}' not found.");
        }

        if (!_accessPolicy.CanAccess(todoList, currentUserId))
        {
            throw new ForbiddenException("You do not have access to this todo list.");
        }

        return ToResponse(todoList);
    }

    public async Task<TodoListResponse> CreateTodoListAsync(CreateTodoListRequest request, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserProvider.GetUserId();
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

        var created = await _todoListRepository.CreateAsync(todoList, cancellationToken);

        return ToResponse(created);
    }

    public async Task DeleteTodoListAsync(string id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserProvider.GetUserId();

        var todoList = await _todoListRepository.GetByIdAsync(id, cancellationToken);

        if (todoList is null)
            throw new NotFoundException($"Todo list '{id}' not found.");

        if (!_accessPolicy.IsOwner(todoList, currentUserId))
            throw new ForbiddenException("Only the owner can delete this todo list.");

        await _todoListRepository.DeleteAsync(id, cancellationToken);
    }

    private static TodoListResponse ToResponse(TodoList todoList) => new()
    {
        Id = todoList.Id,
        Name = todoList.Name,
        OwnerId = todoList.OwnerId,
        CreatedAt = todoList.CreatedAt,
        SharedUserIds = todoList.SharedUserIds
    };
}
