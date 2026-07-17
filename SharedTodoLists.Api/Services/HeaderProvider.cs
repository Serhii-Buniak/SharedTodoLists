using SharedTodoLists.Api.Exceptions;

namespace SharedTodoLists.Api.Services;

public class HeaderProvider(IHttpContextAccessor httpContextAccessor) : IHeaderProvider
{
    public const string UserIdHeaderName = "User-Id";

    public string GetUserId()
    {
        var userId = httpContextAccessor.HttpContext?.Request.Headers[UserIdHeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new HeaderException("Required header 'User-Id' is missing or empty.");
        }

        return userId;
    }
}
