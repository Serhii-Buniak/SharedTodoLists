using Moq;
using SharedTodoLists.Application.Abstractions;
using SharedTodoLists.Application.DTOs.Requests;
using SharedTodoLists.Application.Exceptions;
using SharedTodoLists.Application.Models;
using SharedTodoLists.Application.Services;
using SharedTodoLists.Application.Validation;
using SharedTodoLists.Tests.Mocks;

namespace SharedTodoLists.Tests.Services;

[TestFixture]
public class TodoListServiceUserTests
{
    private Mock<ITodoListRepository> _repository = null!;
    private Mock<ICurrentUserProvider> _currentUserProvider = null!;
    private Mock<ITodoListAccessPolicy> _accessPolicy = null!;
    private Mock<ITodoListValidator> _validator = null!;
    private TodoListService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<ITodoListRepository>();
        _currentUserProvider = new Mock<ICurrentUserProvider>();
        _accessPolicy = new Mock<ITodoListAccessPolicy>();
        _validator = new Mock<ITodoListValidator>();

        _service = new TodoListService(
            _repository.Object,
            _currentUserProvider.Object,
            _accessPolicy.Object,
            _validator.Object);
    }

    // GetTodoListUsersAsync

    [Test]
    public async Task GetTodoListUsersAsync_WhenFoundAndAccessAllowed_ReturnsUsers()
    {
        // Arrange
        var todoList = BuildTodoList(sharedUserIds: new HashSet<string> { "user-2" });
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(todoList);
        _accessPolicy.SetupCanReadReturns(true);

        // Act
        var result = await _service.GetTodoListUsersAsync(todoList.Id);

        // Assert
        Assert.That(result.OwnerId, Is.EqualTo("user-1"));
        Assert.That(result.SharedUserIds, Contains.Item("user-2"));
    }

    [Test]
    public void GetTodoListUsersAsync_WhenNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(null);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => _service.GetTodoListUsersAsync("non-existent-id"));
    }

    [Test]
    public void GetTodoListUsersAsync_WhenAccessDenied_ThrowsForbiddenException()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(BuildTodoList());
        _accessPolicy.SetupCanReadReturns(false);

        // Act & Assert
        Assert.ThrowsAsync<ForbiddenException>(() => _service.GetTodoListUsersAsync("some-id"));
    }

    // AddTodoListUserAsync

    [Test]
    public async Task AddTodoListUserAsync_WhenValid_ReturnsUpdatedUsers()
    {
        // Arrange
        var todoList = BuildTodoList();
        var updated = BuildTodoList(sharedUserIds: new HashSet<string> { "user-2" });
        var request = new AddTodoListUserRequest { UserId = "user-2" };
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(todoList);
        _accessPolicy.SetupCanManageUsersReturns(true);
        _repository.SetupAddUserReturns(updated);

        // Act
        var result = await _service.AddTodoListUserAsync(todoList.Id, request);

        // Assert
        Assert.That(result.SharedUserIds, Contains.Item("user-2"));
    }

    [Test]
    public void AddTodoListUserAsync_WhenNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(null);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() =>
            _service.AddTodoListUserAsync("non-existent-id", new AddTodoListUserRequest { UserId = "user-2" }));
    }

    [Test]
    public void AddTodoListUserAsync_WhenAccessDenied_ThrowsForbiddenException()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(BuildTodoList());
        _accessPolicy.SetupCanManageUsersReturns(false);

        // Act & Assert
        Assert.ThrowsAsync<ForbiddenException>(() =>
            _service.AddTodoListUserAsync("some-id", new AddTodoListUserRequest { UserId = "user-2" }));
    }

    [Test]
    public void AddTodoListUserAsync_WhenAddingOwner_ThrowsBadRequestException()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(BuildTodoList(ownerId: "user-1"));
        _accessPolicy.SetupCanManageUsersReturns(true);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(() =>
            _service.AddTodoListUserAsync("some-id", new AddTodoListUserRequest { UserId = "user-1" }));
    }

    [Test]
    public void AddTodoListUserAsync_WhenUserAlreadyAdded_ThrowsBadRequestException()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(BuildTodoList(sharedUserIds: new HashSet<string> { "user-2" }));
        _accessPolicy.SetupCanManageUsersReturns(true);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(() =>
            _service.AddTodoListUserAsync("some-id", new AddTodoListUserRequest { UserId = "user-2" }));
    }

    // RemoveTodoListUserAsync

    [Test]
    public async Task RemoveTodoListUserAsync_WhenValid_RemovesUser()
    {
        // Arrange
        var todoList = BuildTodoList(sharedUserIds: new HashSet<string> { "user-2" });
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(todoList);
        _accessPolicy.SetupCanManageUsersReturns(true);
        _repository.SetupRemoveUserCompletes();

        // Act
        await _service.RemoveTodoListUserAsync(todoList.Id, "user-2");

        // Assert
        _repository.Verify(r => r.RemoveUserAsync(todoList.Id, "user-2", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void RemoveTodoListUserAsync_WhenNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(null);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => _service.RemoveTodoListUserAsync("non-existent-id", "user-2"));
    }

    [Test]
    public void RemoveTodoListUserAsync_WhenAccessDenied_ThrowsForbiddenException()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(BuildTodoList());
        _accessPolicy.SetupCanManageUsersReturns(false);

        // Act & Assert
        Assert.ThrowsAsync<ForbiddenException>(() => _service.RemoveTodoListUserAsync("some-id", "user-2"));
    }

    [Test]
    public void RemoveTodoListUserAsync_WhenRemovingOwner_ThrowsBadRequestException()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(BuildTodoList(ownerId: "user-1"));
        _accessPolicy.SetupCanManageUsersReturns(true);

        // Act & Assert
        Assert.ThrowsAsync<BadRequestException>(() => _service.RemoveTodoListUserAsync("some-id", "user-1"));
    }

    [Test]
    public void RemoveTodoListUserAsync_WhenUserNotMember_ThrowsNotFoundException()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(BuildTodoList(sharedUserIds: new HashSet<string>()));
        _accessPolicy.SetupCanManageUsersReturns(true);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() => _service.RemoveTodoListUserAsync("some-id", "user-99"));
    }

    private static TodoList BuildTodoList(string ownerId = "user-1", IReadOnlySet<string>? sharedUserIds = null) =>
        new()
        {
            Id = "some-id",
            Name = "Test List",
            OwnerId = ownerId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            SharedUserIds = sharedUserIds ?? new HashSet<string>(),
            Items = []
        };
}