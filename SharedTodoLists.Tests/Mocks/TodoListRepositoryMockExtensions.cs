using Moq;
using SharedTodoLists.Application.Abstractions;
using SharedTodoLists.Application.DTOs.Responses;
using SharedTodoLists.Application.Models;

namespace SharedTodoLists.Tests.Mocks;

internal static class TodoListRepositoryMockExtensions
{
    internal static Mock<ITodoListRepository> SetupGetCursorPageReturns(
        this Mock<ITodoListRepository> mock,
        IReadOnlyList<TodoList> items,
        bool hasMore = false,
        string? nextCursor = null)
    {
        mock.Setup(r => r.GetCursorPageAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CursorResponse<TodoList> { Items = items, HasMore = hasMore, NextCursor = nextCursor });
        return mock;
    }

    internal static Mock<ITodoListRepository> SetupGetPageReturns(
        this Mock<ITodoListRepository> mock,
        IReadOnlyList<TodoListSummary> items,
        long total = 0)
    {
        mock.Setup(r => r.GetPageAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BatchResponse<TodoListSummary> { Items = items, Total = total });
        return mock;
    }

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
        TodoList? todoList = null)
    {
        if (todoList == null)
        {
            mock.Setup(r => r.CreateAsync(It.IsAny<TodoList>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TodoList t, CancellationToken _) => t);
        }
        else
        {
            mock.Setup(r => r.CreateAsync(It.IsAny<TodoList>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(todoList);
        }

        return mock;
    }

    internal static Mock<ITodoListRepository> SetupUpdateReturns(
        this Mock<ITodoListRepository> mock,
        TodoList? todoList = null)
    {
        if (todoList == null)
        {
            mock
                .Setup(x => x.UpdateAsync(It.IsAny<TodoList>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TodoList t, CancellationToken _) => t);
        }
        else
        {
            mock.Setup(r => r.UpdateAsync(It.IsAny<TodoList>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(todoList);
        }
        
        return mock;
    }

    internal static Mock<ITodoListRepository> SetupAddUserReturns(
        this Mock<ITodoListRepository> mock,
        TodoList todoList)
    {
        mock.Setup(r => r.AddUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(todoList);
        return mock;
    }

    internal static Mock<ITodoListRepository> SetupRemoveUserCompletes(
        this Mock<ITodoListRepository> mock)
    {
        mock.Setup(r => r.RemoveUserAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
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
