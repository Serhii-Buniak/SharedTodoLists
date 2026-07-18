using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using SharedTodoLists.Application.Abstractions;
using SharedTodoLists.Persistence.Configs;
using SharedTodoLists.Persistence.Data;
using SharedTodoLists.Persistence.Repositories;

namespace SharedTodoLists.Persistence.Extensions;

public static class PersistenceServiceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, MongoDbOptions mongoOptions)
    {
        services.AddScoped<ITodoListRepository, TodoListRepository>();

        var mongoClient = new MongoClient(mongoOptions.ConnectionString);
        services.AddSingleton<IMongoClient>(mongoClient);

        services.AddSingleton(mongoOptions);

        services.AddSingleton<MongoDbContext>();    
        
        return services;
    }
    
    public static async Task InitializeDataAsync(this IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<MongoDbContext>();

        await context.CreateIndexesAsync(cancellationToken);
    }
}
