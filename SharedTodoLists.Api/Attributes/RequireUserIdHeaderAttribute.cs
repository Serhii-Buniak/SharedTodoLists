using Microsoft.AspNetCore.Mvc.Filters;
using SharedTodoLists.Api.Services;
using SharedTodoLists.Application.Exceptions;

namespace SharedTodoLists.Api.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireUserIdHeaderAttribute : Attribute, IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var userId = context.HttpContext.Request.Headers[HeaderProvider.UserIdHeaderName].FirstOrDefault();

        if (string.IsNullOrWhiteSpace(userId))
            throw new BadRequestException("Required header 'User-Id' is missing or empty.");
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
