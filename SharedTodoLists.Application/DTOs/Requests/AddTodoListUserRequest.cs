using System.ComponentModel.DataAnnotations;

namespace SharedTodoLists.Application.DTOs.Requests;

public record AddTodoListUserRequest
{
    [Required]
    public required string UserId { get; init; }
}
