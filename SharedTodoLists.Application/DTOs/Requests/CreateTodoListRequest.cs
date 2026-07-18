using System.ComponentModel.DataAnnotations;

namespace SharedTodoLists.Application.DTOs.Requests;

public record CreateTodoListRequest
{
    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public required string Name { get; init; }
}
