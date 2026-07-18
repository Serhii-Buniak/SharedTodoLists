using System.ComponentModel.DataAnnotations;

namespace SharedTodoLists.Application.Validation;

public sealed class TodoListNameAttribute : StringLengthAttribute
{
    public TodoListNameAttribute() : base(ValidationConstants.TodoListNameMaxLength)
    {
        MinimumLength = ValidationConstants.TodoListNameMinLength;
    }
}
