using SharedTodoLists.Application.Models;
using SharedTodoLists.Application.Services;

namespace SharedTodoLists.Tests.Policies;

[TestFixture]
public class TodoListAccessPolicyTests
{
    private const string OwnerId = "owner-id";
    private const string SharedUserId = "shared-user-id";
    private const string OtherUserId = "other-user-id";

    private TodoListAccessPolicy _policy = null!;

    [SetUp]
    public void SetUp()
    {
        _policy = new TodoListAccessPolicy();
    }

    [TestCase(OwnerId, ExpectedResult = true, TestName = "Owner")]
    [TestCase(SharedUserId, ExpectedResult = true, TestName = "SharedUser")]
    [TestCase(OtherUserId, ExpectedResult = false, TestName = "UnrelatedUser")]
    public bool CanRead(string userId) => _policy.CanRead(TestTodoList, userId);

    [TestCase(OwnerId, ExpectedResult = true, TestName = "Owner")]
    [TestCase(SharedUserId, ExpectedResult = true, TestName = "SharedUser")]
    [TestCase(OtherUserId, ExpectedResult = false, TestName = "UnrelatedUser")]
    public bool CanUpdate(string userId) => _policy.CanUpdate(TestTodoList, userId);

    [TestCase(OwnerId, ExpectedResult = true, TestName = "Owner")]
    [TestCase(SharedUserId, ExpectedResult = false, TestName = "SharedUser")]
    [TestCase(OtherUserId, ExpectedResult = false, TestName = "UnrelatedUser")]
    public bool CanDelete(string userId) => _policy.CanDelete(TestTodoList, userId);

    [TestCase(OwnerId, ExpectedResult = true, TestName = "Owner")]
    [TestCase(SharedUserId, ExpectedResult = true, TestName = "SharedUser")]
    [TestCase(OtherUserId, ExpectedResult = false, TestName = "UnrelatedUser")]
    public bool CanManageUsers(string userId) => _policy.CanManageUsers(TestTodoList, userId);

    private static TodoList TestTodoList => new()
    {
        Id = "some-id",
        Name = "Test List",
        OwnerId = OwnerId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        SharedUserIds = new HashSet<string> { SharedUserId },
        Items = []
    };
}