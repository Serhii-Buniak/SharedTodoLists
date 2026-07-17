namespace SharedTodoLists.Application.Services;

public interface ITodoListService
{
    Task<string> GetStatusAsync();
}
