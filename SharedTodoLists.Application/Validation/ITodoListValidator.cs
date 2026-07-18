using SharedTodoLists.Application.DTOs.Requests;

namespace SharedTodoLists.Application.Validation;

internal interface ITodoListValidator
{
    void Validate(CreateTodoListRequest request);
    void Validate(UpdateTodoListRequest request);
}
