using SharedTodoLists.Application.Abstractions;
using SharedTodoLists.Application.DTOs.Requests;
using SharedTodoLists.Application.DTOs.Responses;
using SharedTodoLists.Application.Exceptions;
using SharedTodoLists.Application.Models;

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

    public async Task<CursorResponse<TodoListResponse>> GetTodoListsStreamAsync(string? cursor, int limit, bool onlyOwned = false, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserProvider.GetUserId();

        var page = await _todoListRepository.GetCursorPageAsync(currentUserId, cursor, limit, onlyOwned, cancellationToken);

        return new CursorResponse<TodoListResponse>
        {
            Items = page.Items.Select(ToResponse).ToList(),
            NextCursor = page.NextCursor,
            HasMore = page.HasMore
        };
    }

    public async Task<PagedResponse<TodoListResponse>> GetTodoListsAsync(int page, int pageSize, bool onlyOwned = false, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserProvider.GetUserId();

        var batch = await _todoListRepository.GetPageAsync(currentUserId, page, pageSize, onlyOwned, cancellationToken);

        return new PagedResponse<TodoListResponse>
        {
            Items = batch.Items.Select(ToResponse).ToList(),
            Total = batch.Total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<TodoListResponse> GetTodoListAsync(string id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserProvider.GetUserId();

        var todoList = await _todoListRepository.GetByIdAsync(id, cancellationToken);

        if (todoList is null)
            throw new NotFoundException($"Todo list '{id}' not found.");

        if (!_accessPolicy.CanRead(todoList, currentUserId))
            throw new ForbiddenException("You do not have access to this todo list.");

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
            SharedUserIds = new HashSet<string>(),
            Items = []
        };

        var created = await _todoListRepository.CreateAsync(todoList, cancellationToken);

        return ToResponse(created);
    }

    public async Task<TodoListResponse> UpdateTodoListAsync(string id, UpdateTodoListRequest request, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserProvider.GetUserId();

        var todoList = await _todoListRepository.GetByIdAsync(id, cancellationToken);

        if (todoList is null)
            throw new NotFoundException($"Todo list '{id}' not found.");

        if (!_accessPolicy.CanUpdate(todoList, currentUserId))
            throw new ForbiddenException("You do not have access to this todo list.");

        var updated = todoList with
        {
            Name = request.Name,
            Items = request.Items.Select(i => new TodoItem { Name = i.Name, IsDone = i.IsDone }).ToList(),
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _todoListRepository.UpdateAsync(updated, cancellationToken);

        return ToResponse(result);
    }

    public async Task DeleteTodoListAsync(string id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserProvider.GetUserId();

        var todoList = await _todoListRepository.GetByIdAsync(id, cancellationToken);

        if (todoList is null)
            throw new NotFoundException($"Todo list '{id}' not found.");

        if (!_accessPolicy.CanDelete(todoList, currentUserId))
            throw new ForbiddenException("You do not have access to this todo list.");

        await _todoListRepository.DeleteAsync(id, cancellationToken);
    }

    private static TodoListResponse ToResponse(TodoList todoList) => new()
    {
        Id = todoList.Id,
        Name = todoList.Name,
        OwnerId = todoList.OwnerId,
        CreatedAt = todoList.CreatedAt,
        SharedUserIds = todoList.SharedUserIds,
        Items = todoList.Items
    };
}
