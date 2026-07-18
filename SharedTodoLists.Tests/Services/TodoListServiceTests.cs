using Moq;
using SharedTodoLists.Application.Abstractions;
using SharedTodoLists.Application.DTOs.Requests;
using SharedTodoLists.Application.Exceptions;
using SharedTodoLists.Application.Models;
using SharedTodoLists.Application.Services;
using SharedTodoLists.Tests.Mocks;

namespace SharedTodoLists.Tests.Services;

[TestFixture]
public class TodoListServiceTests
{
    private Mock<ITodoListRepository> _repository = null!;
    private Mock<ICurrentUserProvider> _currentUserProvider = null!;
    private Mock<ITodoListAccessPolicy> _accessPolicy = null!;
    private TodoListService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = new Mock<ITodoListRepository>();
        _currentUserProvider = new Mock<ICurrentUserProvider>();
        _accessPolicy = new Mock<ITodoListAccessPolicy>();

        _service = new TodoListService(
            _repository.Object,
            _currentUserProvider.Object,
            _accessPolicy.Object);
    }

    // GetTodoListAsync

    [Test]
    public async Task GetTodoListAsync_WhenFoundAndAccessAllowed_ReturnsTodoList()
    {
        // Arrange
        var todoList = BuildTodoList();
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(todoList);
        _accessPolicy.SetupCanReadReturns(true);

        // Act
        var result = await _service.GetTodoListAsync(todoList.Id);

        // Assert
        Assert.That(result.Id, Is.EqualTo(todoList.Id));
        Assert.That(result.Name, Is.EqualTo(todoList.Name));
    }

    [Test]
    public void GetTodoListAsync_WhenNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(null);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() =>
            _service.GetTodoListAsync("non-existent-id"));
    }

    [Test]
    public void GetTodoListAsync_WhenAccessDenied_ThrowsForbiddenException()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(BuildTodoList());
        _accessPolicy.SetupCanReadReturns(false);

        // Act & Assert
        Assert.ThrowsAsync<ForbiddenException>(() =>
            _service.GetTodoListAsync("some-id"));
    }

    // CreateTodoListAsync

    [Test]
    public async Task CreateTodoListAsync_WhenCalled_ReturnsTodoListWithCurrentUserAsOwner()
    {
        // Arrange
        const string currentUserId = "user-1";
        var request = new CreateTodoListRequest { Name = "My List" };
        var created = BuildTodoList(ownerId: currentUserId);
        _currentUserProvider.SetupGetUserIdReturns(currentUserId);
        _repository.SetupCreateReturns(created);

        // Act
        var result = await _service.CreateTodoListAsync(request);

        // Assert
        Assert.That(result.OwnerId, Is.EqualTo(currentUserId));
        Assert.That(result.Name, Is.EqualTo(created.Name));
    }

    // DeleteTodoListAsync

    [Test]
    public async Task DeleteTodoListAsync_WhenOwner_DeletesSuccessfully()
    {
        // Arrange
        var todoList = BuildTodoList();
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(todoList);
        _accessPolicy.SetupCanDeleteReturns(true);
        _repository.SetupDeleteCompletes();

        // Act
        await _service.DeleteTodoListAsync(todoList.Id);

        // Assert
        _repository.Verify(r => r.DeleteAsync(todoList.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Test]
    public void DeleteTodoListAsync_WhenNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(null);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() =>
            _service.DeleteTodoListAsync("non-existent-id"));
    }

    [Test]
    public void DeleteTodoListAsync_WhenAccessDenied_ThrowsForbiddenException()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(BuildTodoList());
        _accessPolicy.SetupCanDeleteReturns(false);

        // Act & Assert
        Assert.ThrowsAsync<ForbiddenException>(() =>
            _service.DeleteTodoListAsync("some-id"));
    }

    private static TodoList BuildTodoList(string ownerId = "user-1") => new()
    {
        Id = "some-id",
        Name = "Test List",
        OwnerId = ownerId,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        SharedUserIds = []
    };
}
