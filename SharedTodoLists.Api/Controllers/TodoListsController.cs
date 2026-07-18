using Microsoft.AspNetCore.Mvc;
using SharedTodoLists.Api.Attributes;
using SharedTodoLists.Application.DTOs.Requests;
using SharedTodoLists.Application.Services;

namespace SharedTodoLists.Api.Controllers;

[ApiController]
[Route("api/todo-lists")]
[RequireUserIdHeader]
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
    public async Task<IActionResult> GetTodoList([FromRoute] string id, CancellationToken cancellationToken)
    {
        var result = await todoListService.GetTodoListAsync(id, cancellationToken);
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTodoList([FromBody] CreateTodoListRequest request, CancellationToken cancellationToken)
    {
        var result = await todoListService.CreateTodoListAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetTodoList), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public IActionResult UpdateTodoList([FromRoute] string id)
    {
        throw new NotImplementedException();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTodoList([FromRoute] string id)
    {
        throw new NotImplementedException();
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
