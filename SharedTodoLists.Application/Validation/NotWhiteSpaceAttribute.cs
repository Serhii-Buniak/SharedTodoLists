using System.ComponentModel.DataAnnotations;

namespace SharedTodoLists.Application.Validation;

public sealed class NotWhiteSpaceAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        return value is string s && !string.IsNullOrWhiteSpace(s)
            ? ValidationResult.Success
            : new ValidationResult($"{validationContext.DisplayName} cannot be empty or whitespace.");
    }
}
