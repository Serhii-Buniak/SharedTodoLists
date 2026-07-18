using SharedTodoLists.Application.Models;
using SharedTodoLists.Application.Policies;

namespace SharedTodoLists.Tests.Policies;

[TestFixture]
public class TodoListAccessPolicyTests
{
    private TodoListAccessPolicy _policy = null!;

    [SetUp]
    public void SetUp()
    {
        _policy = new TodoListAccessPolicy();
    }

    // Rule: get list of lists returns only lists where user is owner OR has a shared relation

    [Test]
    public void CanAccess_WhenUserIsOwner_ReturnsTrue()
    {
        var todoList = BuildTodoList(ownerId: "user-1");

        Assert.That(_policy.CanAccess(todoList, "user-1"), Is.True);
    }

    [Test]
    public void CanAccess_WhenUserIsShared_ReturnsTrue()
    {
        var todoList = BuildTodoList(ownerId: "user-1", sharedUserIds: ["user-2"]);

        Assert.That(_policy.CanAccess(todoList, "user-2"), Is.True);
    }

    [Test]
    public void CanAccess_WhenUserHasNoRelation_ReturnsFalse()
    {
        var todoList = BuildTodoList(ownerId: "user-1", sharedUserIds: ["user-2"]);

        Assert.That(_policy.CanAccess(todoList, "user-3"), Is.False);
    }

    // Rule: get, update, add/get/remove user relations — owner OR shared user

    [Test]
    public void CanAccess_Owner_CanGetTodoList() =>
        Assert.That(_policy.CanAccess(BuildTodoList(ownerId: "user-1"), "user-1"), Is.True);

    [Test]
    public void CanAccess_SharedUser_CanGetTodoList() =>
        Assert.That(_policy.CanAccess(BuildTodoList(ownerId: "user-1", sharedUserIds: ["user-2"]), "user-2"), Is.True);

    [Test]
    public void CanAccess_UnrelatedUser_CannotGetTodoList() =>
        Assert.That(_policy.CanAccess(BuildTodoList(ownerId: "user-1"), "user-2"), Is.False);

    [Test]
    public void CanAccess_Owner_CanUpdateTodoList() =>
        Assert.That(_policy.CanAccess(BuildTodoList(ownerId: "user-1"), "user-1"), Is.True);

    [Test]
    public void CanAccess_SharedUser_CanUpdateTodoList() =>
        Assert.That(_policy.CanAccess(BuildTodoList(ownerId: "user-1", sharedUserIds: ["user-2"]), "user-2"), Is.True);

    [Test]
    public void CanAccess_UnrelatedUser_CannotUpdateTodoList() =>
        Assert.That(_policy.CanAccess(BuildTodoList(ownerId: "user-1"), "user-2"), Is.False);

    [Test]
    public void CanAccess_Owner_CanManageUserRelations() =>
        Assert.That(_policy.CanAccess(BuildTodoList(ownerId: "user-1"), "user-1"), Is.True);

    [Test]
    public void CanAccess_SharedUser_CanManageUserRelations() =>
        Assert.That(_policy.CanAccess(BuildTodoList(ownerId: "user-1", sharedUserIds: ["user-2"]), "user-2"), Is.True);

    [Test]
    public void CanAccess_UnrelatedUser_CannotManageUserRelations() =>
        Assert.That(_policy.CanAccess(BuildTodoList(ownerId: "user-1"), "user-2"), Is.False);

    // Rule: delete — only the owner

    [Test]
    public void IsOwner_Owner_CanDeleteTodoList() =>
        Assert.That(_policy.IsOwner(BuildTodoList(ownerId: "user-1"), "user-1"), Is.True);

    [Test]
    public void IsOwner_SharedUser_CannotDeleteTodoList() =>
        Assert.That(_policy.IsOwner(BuildTodoList(ownerId: "user-1", sharedUserIds: ["user-2"]), "user-2"), Is.False);

    [Test]
    public void IsOwner_UnrelatedUser_CannotDeleteTodoList() =>
        Assert.That(_policy.IsOwner(BuildTodoList(ownerId: "user-1"), "user-2"), Is.False);

    private static TodoList BuildTodoList(string ownerId, HashSet<string>? sharedUserIds = null) => new()
    {
        Id = "some-id",
        Name = "Test List",
        OwnerId = ownerId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        SharedUserIds = sharedUserIds ?? []
    };
}
