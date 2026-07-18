namespace SharedTodoLists.Api.Extensions;

public static class ConfigurationExtensions
{
    public static T GetRequiredOptions<T>(this IConfiguration configuration, string sectionName) where T : class
    {
        ArgumentNullException.ThrowIfNull(configuration);

        return configuration
                   .GetSection(sectionName)
                   .Get<T>()
               ?? throw new InvalidOperationException(
                   $"Configuration section '{sectionName}' is missing or invalid.");
    }

    public static string GetRequiredOptions(this IConfiguration configuration, string sectionName)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        var result = configuration
            .GetSection(sectionName)
            .Get<string>();

        if (string.IsNullOrWhiteSpace(result))
        {
            throw new InvalidOperationException(
                $"Configuration section '{sectionName}' is missing or invalid.");
        }

        return result;
    }
}