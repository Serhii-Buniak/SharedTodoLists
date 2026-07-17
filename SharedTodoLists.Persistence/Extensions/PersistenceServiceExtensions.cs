using Microsoft.Extensions.DependencyInjection;
using SharedTodoLists.Application.Abstractions.Repositories;
using SharedTodoLists.Persistence.Repositories;

namespace SharedTodoLists.Persistence.Extensions;

public static class PersistenceServiceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services)
    {
        services.AddScoped<ITodoListRepository, TodoListRepository>();

        return services;
    }
}
