using Moq;
using SharedTodoLists.Application.Abstractions;

namespace SharedTodoLists.Tests.Mocks;

internal static class CurrentUserProviderMockExtensions
{
    internal static Mock<ICurrentUserProvider> SetupGetUserIdReturns(
        this Mock<ICurrentUserProvider> mock,
        string userId)
    {
        mock.Setup(p => p.GetUserId()).Returns(userId);
        return mock;
    }
}
