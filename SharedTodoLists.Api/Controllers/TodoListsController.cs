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
    public IActionResult GetTodoLists(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TodoListResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetTodoList([FromRoute] string id, CancellationToken cancellationToken)
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
    public IActionResult GetTodoListUsers([FromRoute] string id)
    {
        throw new NotImplementedException();
    }

    [HttpPost("{id}/users")]
    public IActionResult AddTodoListUser([FromRoute] string id)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}/users/{userId}")]
    public IActionResult RemoveTodoListUser([FromRoute] string id, [FromRoute] string userId)
    {
        throw new NotImplementedException();
    }
}
