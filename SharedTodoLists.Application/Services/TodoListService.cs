using SharedTodoLists.Application.Abstractions;
using SharedTodoLists.Application.DTOs.Requests;
using SharedTodoLists.Application.DTOs.Responses;
using SharedTodoLists.Application.Exceptions;
using SharedTodoLists.Application.Extensions;
using SharedTodoLists.Application.Models;
using SharedTodoLists.Application.Validation;

namespace SharedTodoLists.Application.Services;

internal class TodoListService : ITodoListService
{
    private readonly ITodoListRepository _todoListRepository;
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly ITodoListAccessPolicy _accessPolicy;
    private readonly ITodoListValidator _validator;

    public TodoListService(ITodoListRepository todoListRepository,
        ICurrentUserProvider currentUserProvider,
        ITodoListAccessPolicy accessPolicy,
        ITodoListValidator validator)
    {
        _todoListRepository = todoListRepository;
        _currentUserProvider = currentUserProvider;
        _accessPolicy = accessPolicy;
        _validator = validator;
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

    public async Task<PagedResponse<TodoListSummary>> GetTodoListsAsync(int page, int pageSize, bool onlyOwned = false, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserProvider.GetUserId();

        var batch = await _todoListRepository.GetPageAsync(currentUserId, page, pageSize, onlyOwned, cancellationToken);

        return new PagedResponse<TodoListSummary>
        {
            Items = batch.Items,
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

        _accessPolicy.EnsureCanRead(todoList, currentUserId);

        return ToResponse(todoList);
    }

    public async Task<TodoListResponse> CreateTodoListAsync(CreateTodoListRequest request, CancellationToken cancellationToken = default)
    {
        _validator.Validate(request);
        
        var currentUserId = _currentUserProvider.GetUserId();
        var now = DateTime.UtcNow;

        var todoList = new TodoList
        {
            Id = string.Empty,
            Name = Normalize(request.Name),
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
        _validator.Validate(request);
        
        var currentUserId = _currentUserProvider.GetUserId();

        var todoList = await _todoListRepository.GetByIdAsync(id, cancellationToken);

        if (todoList is null)
            throw new NotFoundException($"Todo list '{id}' not found.");

        _accessPolicy.EnsureCanUpdate(todoList, currentUserId);

        var updated = todoList with
        {
            Name = Normalize(request.Name),
            Items = request.Items.Select(i => new TodoItem
            {
                Name = Normalize(i.Name),
                IsDone = i.IsDone
            }).ToList(),
            UpdatedAt = DateTime.UtcNow
        };

        var result = await _todoListRepository.UpdateAsync(updated, cancellationToken);

        return ToResponse(result);
    }

    public async Task<TodoListUsersResponse> GetTodoListUsersAsync(string id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserProvider.GetUserId();

        var todoList = await _todoListRepository.GetByIdAsync(id, cancellationToken);

        if (todoList is null)
            throw new NotFoundException($"Todo list '{id}' not found.");

        _accessPolicy.EnsureCanRead(todoList, currentUserId);

        return ToUsersResponse(todoList);
    }

    public async Task<TodoListUsersResponse> AddTodoListUserAsync(string id, AddTodoListUserRequest request, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserProvider.GetUserId();

        var todoList = await _todoListRepository.GetByIdAsync(id, cancellationToken);

        if (todoList is null)
            throw new NotFoundException($"Todo list '{id}' not found.");

        _accessPolicy.EnsureCanManageUsers(todoList, currentUserId);

        if (request.UserId == todoList.OwnerId)
            throw new BadRequestException("Cannot add the owner as a shared user.");

        if (todoList.SharedUserIds.Contains(request.UserId))
            throw new BadRequestException("User is already added to this todo list.");

        var updated = await _todoListRepository.AddUserAsync(id, request.UserId, cancellationToken);

        return ToUsersResponse(updated);
    }

    public async Task RemoveTodoListUserAsync(string id, string userId, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserProvider.GetUserId();

        var todoList = await _todoListRepository.GetByIdAsync(id, cancellationToken);

        if (todoList is null)
            throw new NotFoundException($"Todo list '{id}' not found.");

        _accessPolicy.EnsureCanManageUsers(todoList, currentUserId);

        if (userId == todoList.OwnerId)
            throw new BadRequestException("Cannot remove the owner from the todo list.");

        if (!todoList.SharedUserIds.Contains(userId))
            throw new NotFoundException($"User '{userId}' is not a member of this todo list.");

        await _todoListRepository.RemoveUserAsync(id, userId, cancellationToken);
    }

    public async Task DeleteTodoListAsync(string id, CancellationToken cancellationToken = default)
    {
        var currentUserId = _currentUserProvider.GetUserId();

        var todoList = await _todoListRepository.GetByIdAsync(id, cancellationToken);

        if (todoList is null)
            throw new NotFoundException($"Todo list '{id}' not found.");

        _accessPolicy.EnsureCanDelete(todoList, currentUserId);

        await _todoListRepository.DeleteAsync(id, cancellationToken);
    }
    
    private static string Normalize(string value) => value.Trim();

    private static TodoListUsersResponse ToUsersResponse(TodoList todoList) => new()
    {
        OwnerId = todoList.OwnerId,
        SharedUserIds = todoList.SharedUserIds
    };

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
