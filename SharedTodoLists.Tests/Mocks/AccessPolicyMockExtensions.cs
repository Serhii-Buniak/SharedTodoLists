using Moq;
using SharedTodoLists.Application.Models;
using SharedTodoLists.Application.Services;

namespace SharedTodoLists.Tests.Mocks;

internal static class AccessPolicyMockExtensions
{
    internal static Mock<ITodoListAccessPolicy> SetupCanReadReturns(
        this Mock<ITodoListAccessPolicy> mock,
        bool result)
    {
        mock.Setup(p => p.CanRead(It.IsAny<TodoList>(), It.IsAny<string>()))
            .Returns(result);
        return mock;
    }

    internal static Mock<ITodoListAccessPolicy> SetupCanUpdateReturns(
        this Mock<ITodoListAccessPolicy> mock,
        bool result)
    {
        mock.Setup(p => p.CanUpdate(It.IsAny<TodoList>(), It.IsAny<string>()))
            .Returns(result);
        return mock;
    }

    internal static Mock<ITodoListAccessPolicy> SetupCanDeleteReturns(
        this Mock<ITodoListAccessPolicy> mock,
        bool result)
    {
        mock.Setup(p => p.CanDelete(It.IsAny<TodoList>(), It.IsAny<string>()))
            .Returns(result);
        return mock;
    }
}
