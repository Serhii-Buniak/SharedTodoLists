using SharedTodoLists.Application.Abstractions;
using SharedTodoLists.Application.Exceptions;

namespace SharedTodoLists.Api.Services;

public class HeaderProvider(IHttpContextAccessor httpContextAccessor) : ICurrentUserProvider
{
    public const string UserIdHeaderName = "User-Id";

    public string GetUserId()
    {
        var userId = httpContextAccessor.HttpContext?.Request.Headers[UserIdHeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new BadRequestException("Required header 'User-Id' is missing or empty.");
        }

        return userId;
    }
}