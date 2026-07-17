using SharedTodoLists.Application.Abstractions.Repositories;

namespace SharedTodoLists.Application.Services;

internal class TodoListService : ITodoListService
{
    public TodoListService(ITodoListRepository todoListRepository)
    {
    }

    public Task<string> GetStatusAsync()
    {
        return Task.FromResult("Application layer is working");
    }
}
