namespace SharedTodoLists.Application.Abstractions;

public interface ICurrentUserProvider
{
    string GetUserId();
}