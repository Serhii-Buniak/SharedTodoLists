using Microsoft.Extensions.DependencyInjection;
using SharedTodoLists.Application.Services;

namespace SharedTodoLists.Application.Extensions;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITodoListAccessPolicy, TodoListAccessPolicy>();
        services.AddScoped<ITodoListService, TodoListService>();
        return services;
    }
}
