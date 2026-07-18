using System.ComponentModel.DataAnnotations;

namespace SharedTodoLists.Application.DTOs.Requests;

public record UpdateTodoListRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public required string Name { get; init; }

    public required IReadOnlyList<TodoItemRequest> Items { get; init; }
}

public record TodoItemRequest
{
    [Required]
    [MinLength(1)]
    public required string Name { get; init; }

    public required bool IsDone { get; init; }
}
