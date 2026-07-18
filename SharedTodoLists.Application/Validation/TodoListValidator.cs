using SharedTodoLists.Application.DTOs.Requests;
using SharedTodoLists.Application.Exceptions;

namespace SharedTodoLists.Application.Validation;

internal class TodoListValidator : ITodoListValidator
{
    public void Validate(CreateTodoListRequest request)
    {
        ValidateName(request.Name);
    }

    public void Validate(UpdateTodoListRequest request)
    {
        ValidateName(request.Name);

        foreach (var item in request.Items)
            ValidateItem(item);
    }

    private static void ValidateName(string name)
    {
        name = name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new BadRequestException("Todo list name cannot be empty.");

        if (name.Length > 255)
            throw new BadRequestException("Todo list name cannot exceed 255 characters.");
    }

    private static void ValidateItem(TodoItemRequest item)
    {
        var name = item.Name.Trim();

        if (string.IsNullOrWhiteSpace(name))
            throw new BadRequestException("Todo item name cannot be empty.");
    }
}