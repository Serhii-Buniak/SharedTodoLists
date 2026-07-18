using Microsoft.AspNetCore.Mvc;
using SharedTodoLists.Api.Attributes;
using SharedTodoLists.Application.DTOs.Requests;
using SharedTodoLists.Application.DTOs.Responses;
using SharedTodoLists.Application.Services;

namespace SharedTodoLists.Api.Controllers;

[ApiController]
[Route("api/todo-lists")]
[RequireUserIdHeader]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
public class TodoListsController(ITodoListService todoListService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<TodoListSummary>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<TodoListSummary>>> GetTodoLists(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool onlyOwned = false,
        CancellationToken cancellationToken = default)
    {
        var result = await todoListService.GetTodoListsAsync(page, pageSize, onlyOwned, cancellationToken);
        return Ok(result);
    }

    [HttpGet("stream")]
    [ProducesResponseType(typeof(CursorResponse<TodoListResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<CursorResponse<TodoListResponse>>> GetTodoListsStream(
        [FromQuery] string? cursor = null,
        [FromQuery] int limit = 20,
        [FromQuery] bool onlyOwned = false,
        CancellationToken cancellationToken = default)
    {
        var result = await todoListService.GetTodoListsStreamAsync(cursor, limit, onlyOwned, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TodoListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TodoListResponse>> GetTodoList([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await todoListService.GetTodoListAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TodoListResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateTodoList([FromBody] CreateTodoListRequest request, CancellationToken cancellationToken)
    {
        var result = await todoListService.CreateTodoListAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetTodoList), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TodoListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> UpdateTodoList(
        [FromRoute] string id,
        [FromBody] UpdateTodoListRequest request,
        CancellationToken cancellationToken)
    {
        var result = await todoListService.UpdateTodoListAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> DeleteTodoList([FromRoute] string id, CancellationToken cancellationToken)
    {
        await todoListService.DeleteTodoListAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("{id}/users")]
    [ProducesResponseType(typeof(TodoListUsersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TodoListUsersResponse>> GetTodoListUsers([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await todoListService.GetTodoListUsersAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id}/users")]
    [ProducesResponseType(typeof(TodoListUsersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<TodoListUsersResponse>> AddTodoListUser(
        [FromRoute] string id,
        [FromBody] AddTodoListUserRequest request,
        CancellationToken cancellationToken)
    {
        var result = await todoListService.AddTodoListUserAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id}/users/{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveTodoListUser(
        [FromRoute] string id,
        [FromRoute] string userId,
        CancellationToken cancellationToken)
    {
        await todoListService.RemoveTodoListUserAsync(id, userId, cancellationToken);
        return NoContent();
    }
}
