using Microsoft.AspNetCore.Mvc;
using SharedTodoLists.Api.Attributes;
using SharedTodoLists.Api.Services;

namespace SharedTodoLists.Api.Controllers;

[ApiController]
[Route("api/todo-lists")]
[RequireUserIdHeader]
public class TodoListsController : ControllerBase
{
    private readonly IHeaderProvider _headerProvider;

    public TodoListsController(IHeaderProvider headerProvider)
    {
        _headerProvider = headerProvider;
    }
    
    [HttpGet]
    public IActionResult GetTodoLists(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{id}")]
    public IActionResult GetTodoList([FromRoute] string id)
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public IActionResult CreateTodoList()
    {
        throw new NotImplementedException();
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
