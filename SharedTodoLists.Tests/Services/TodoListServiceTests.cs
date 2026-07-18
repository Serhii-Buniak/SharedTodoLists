using Moq;
using SharedTodoLists.Application.Abstractions;
using SharedTodoLists.Application.DTOs.Requests;
using SharedTodoLists.Application.DTOs.Responses;
using SharedTodoLists.Application.Exceptions;
using SharedTodoLists.Application.Models;
using SharedTodoLists.Application.Services;
using SharedTodoLists.Application.Validation;
using SharedTodoLists.Tests.Mocks;

namespace SharedTodoLists.Tests.Services;

[TestFixture]
public class TodoListServiceTests
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

    // GetTodoListsStreamAsync

    [Test]
    public async Task GetTodoListsStreamAsync_WhenCalled_ReturnsMappedItems()
    {
        // Arrange
        var todoLists = new List<TodoList> { BuildTodoList(), BuildTodoList() };
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetCursorPageReturns(todoLists, hasMore: false);

        // Act
        var result = await _service.GetTodoListsStreamAsync(cursor: null, limit: 20);

        // Assert
        Assert.That(result.Items, Has.Count.EqualTo(2));
        Assert.That(result.HasMore, Is.False);
        Assert.That(result.NextCursor, Is.Null);
    }

    [Test]
    public async Task GetTodoListsStreamAsync_WhenHasMore_ReturnsNextCursor()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetCursorPageReturns([BuildTodoList()], hasMore: true, nextCursor: "cursor-abc");

        // Act
        var result = await _service.GetTodoListsStreamAsync(cursor: null, limit: 1);

        // Assert
        Assert.That(result.HasMore, Is.True);
        Assert.That(result.NextCursor, Is.EqualTo("cursor-abc"));
    }

    // GetTodoListsAsync

    [Test]
    public async Task GetTodoListsAsync_WhenCalled_ReturnsPagedResult()
    {
        // Arrange
        var summaries = new List<TodoListSummary>
        {
            new() { Id = "id-1", Name = "List 1" },
            new() { Id = "id-2", Name = "List 2" }
        };
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetPageReturns(summaries, total: 5);

        // Act
        var result = await _service.GetTodoListsAsync(page: 1, pageSize: 2);

        // Assert
        Assert.That(result.Items, Has.Count.EqualTo(2));
        Assert.That(result.Total, Is.EqualTo(5));
        Assert.That(result.Page, Is.EqualTo(1));
        Assert.That(result.PageSize, Is.EqualTo(2));
    }

    [Test]
    public async Task GetTodoListsAsync_WhenNoLists_ReturnsEmptyPage()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetPageReturns([], total: 0);

        // Act
        var result = await _service.GetTodoListsAsync(page: 1, pageSize: 20);

        // Assert
        Assert.That(result.Items, Is.Empty);
        Assert.That(result.Total, Is.EqualTo(0));
    }

    [Test]
    public async Task GetTodoListsAsync_WhenOnlyOwned_PassesFlagToRepository()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetPageReturns([], total: 0);

        // Act
        await _service.GetTodoListsAsync(page: 1, pageSize: 20, onlyOwned: true);

        // Assert
        _repository.Verify(r => r.GetPageAsync("user-1", 1, 20, true, It.IsAny<CancellationToken>()), Times.Once);
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
    [TestCase("My List", TestName = "Normalized Name")]
    [TestCase(" My List ", TestName = "Not Normalized Name")]
    public async Task CreateTodoListAsync_WhenCalled_ReturnsTodoListWithCurrentUserAsOwner(string requestName)
    {
        // Arrange
        const string currentUserId = "user-1";
        var request = new CreateTodoListRequest { Name = requestName };
        _currentUserProvider.SetupGetUserIdReturns(currentUserId);
        _repository.SetupCreateReturns();

        // Act
        var result = await _service.CreateTodoListAsync(request);

        // Assert
        Assert.That(result.OwnerId, Is.EqualTo(currentUserId));
        Assert.That(result.Name, Is.EqualTo("My List"));
    }

    // UpdateTodoListAsync

    [Test]
    [TestCase("Updated Name", "Task 1", TestName = "Normalized Name")]
    [TestCase(" Updated Name ", " Task 1 ", TestName = "Not Normalized Name")]
    public async Task UpdateTodoListAsync_WhenFoundAndAccessAllowed_ReturnsUpdatedTodoList(string name, string taskName)
    {
        // Arrange
        var todoList = BuildTodoList();
        var request = new UpdateTodoListRequest
        {
            Name = name,
            Items = [new TodoItemRequest { Name = taskName , IsDone = false }]
        };
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(todoList);
        _accessPolicy.SetupCanUpdateReturns(true);
        _repository.SetupUpdateReturns();

        // Act
        var result = await _service.UpdateTodoListAsync(todoList.Id, request);

        // Assert
        Assert.That(result.Name, Is.EqualTo("Updated Name"));
        Assert.That(result.Items[0].Name, Is.EqualTo("Task 1"));
        Assert.That(result.Items, Has.Count.EqualTo(1));
    }

    [Test]
    public void UpdateTodoListAsync_WhenNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(null);

        // Act & Assert
        Assert.ThrowsAsync<NotFoundException>(() =>
            _service.UpdateTodoListAsync("non-existent-id", new UpdateTodoListRequest { Name = "Name", Items = [] }));
    }

    [Test]
    public void UpdateTodoListAsync_WhenAccessDenied_ThrowsForbiddenException()
    {
        // Arrange
        _currentUserProvider.SetupGetUserIdReturns("user-1");
        _repository.SetupGetByIdReturns(BuildTodoList());
        _accessPolicy.SetupCanUpdateReturns(false);

        // Act & Assert
        Assert.ThrowsAsync<ForbiddenException>(() =>
            _service.UpdateTodoListAsync("some-id", new UpdateTodoListRequest { Name = "Name", Items = [] }));
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
        SharedUserIds = new HashSet<string>(),
        Items = []
    };
}
