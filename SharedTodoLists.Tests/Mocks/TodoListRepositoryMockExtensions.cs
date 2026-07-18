using Moq;
using SharedTodoLists.Application.Abstractions;
using SharedTodoLists.Application.Models;

namespace SharedTodoLists.Tests.Mocks;

internal static class TodoListRepositoryMockExtensions
{
    internal static Mock<ITodoListRepository> SetupGetByIdReturns(
        this Mock<ITodoListRepository> mock,
        TodoList? todoList)
    {
        mock.Setup(r => r.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(todoList);
        return mock;
    }

    internal static Mock<ITodoListRepository> SetupCreateReturns(
        this Mock<ITodoListRepository> mock,
        TodoList todoList)
    {
        mock.Setup(r => r.CreateAsync(It.IsAny<TodoList>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(todoList);
        return mock;
    }

    internal static Mock<ITodoListRepository> SetupUpdateReturns(
        this Mock<ITodoListRepository> mock,
        TodoList todoList)
    {
        mock.Setup(r => r.UpdateAsync(It.IsAny<TodoList>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(todoList);
        return mock;
    }

    internal static Mock<ITodoListRepository> SetupDeleteCompletes(
        this Mock<ITodoListRepository> mock)
    {
        mock.Setup(r => r.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        return mock;
    }
}
