using Moq;
using TodoList.Application.DTOs;
using TodoList.Application.Services;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;
using Xunit;

namespace TodoList.Application.Tests.Services;

public class TodoServiceTests
{
    private readonly Mock<ITodoRepository> _mockRepository;
    private readonly TodoService _service;

    public TodoServiceTests()
    {
        _mockRepository = new Mock<ITodoRepository>();
        _service = new TodoService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAllTodosAsync_ReturnsAllTodos()
    {
        var todos = new List<TodoItem>
        {
            new TodoItem("Todo 1"),
            new TodoItem("Todo 2")
        };
        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(todos);

        var result = await _service.GetAllTodosAsync();

        Assert.Equal(2, result.Count());
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetTodoByIdAsync_WithExistingId_ReturnsTodo()
    {
        var todo = new TodoItem("Test Todo");
        _mockRepository.Setup(r => r.GetByIdAsync(todo.Id)).ReturnsAsync(todo);

        var result = await _service.GetTodoByIdAsync(todo.Id);

        Assert.NotNull(result);
        Assert.Equal(todo.Title, result.Title);
        _mockRepository.Verify(r => r.GetByIdAsync(todo.Id), Times.Once);
    }

    [Fact]
    public async Task GetTodoByIdAsync_WithNonExistingId_ReturnsNull()
    {
        var id = Guid.NewGuid();
        _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((TodoItem?)null);

        var result = await _service.GetTodoByIdAsync(id);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateTodoAsync_WithValidDto_CreatesTodo()
    {
        var dto = new CreateTodoDto("New Todo", "Description", DateTime.UtcNow.AddDays(7));
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<TodoItem>()))
            .ReturnsAsync((TodoItem item) => item);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.CreateTodoAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.Title, result.Title);
        Assert.Equal(dto.Description, result.Description);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<TodoItem>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task CreateTodoAsync_WithInvalidTitle_ThrowsArgumentException(string invalidTitle)
    {
        var dto = new CreateTodoDto(invalidTitle);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateTodoAsync(dto));
    }

    [Fact]
    public async Task UpdateTodoAsync_WithExistingId_UpdatesTodo()
    {
        var todo = new TodoItem("Original Title");
        var dto = new UpdateTodoDto(Title: "Updated Title");
        _mockRepository.Setup(r => r.GetByIdAsync(todo.Id)).ReturnsAsync(todo);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<TodoItem>()))
            .ReturnsAsync((TodoItem item) => item);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.UpdateTodoAsync(todo.Id, dto);

        Assert.Equal("Updated Title", result.Title);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<TodoItem>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateTodoAsync_WithNonExistingId_ThrowsKeyNotFoundException()
    {
        var id = Guid.NewGuid();
        var dto = new UpdateTodoDto(Title: "Updated Title");
        _mockRepository.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((TodoItem?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.UpdateTodoAsync(id, dto));
    }

    [Fact]
    public async Task MarkAsCompletedAsync_WithExistingId_MarksAsCompleted()
    {
        var todo = new TodoItem("Test Todo");
        _mockRepository.Setup(r => r.GetByIdAsync(todo.Id)).ReturnsAsync(todo);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<TodoItem>()))
            .ReturnsAsync((TodoItem item) => item);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.MarkAsCompletedAsync(todo.Id);

        Assert.True(result.IsCompleted);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<TodoItem>()), Times.Once);
    }

    [Fact]
    public async Task MarkAsIncompleteAsync_WithExistingId_MarksAsIncomplete()
    {
        var todo = new TodoItem("Test Todo");
        todo.MarkAsCompleted();
        _mockRepository.Setup(r => r.GetByIdAsync(todo.Id)).ReturnsAsync(todo);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<TodoItem>()))
            .ReturnsAsync((TodoItem item) => item);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.MarkAsIncompleteAsync(todo.Id);

        Assert.False(result.IsCompleted);
    }

    [Fact]
    public async Task DeleteTodoAsync_WithExistingId_DeletesTodo()
    {
        var id = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(id)).ReturnsAsync(true);
        _mockRepository.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var result = await _service.DeleteTodoAsync(id);

        Assert.True(result);
        _mockRepository.Verify(r => r.DeleteAsync(id), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteTodoAsync_WithNonExistingId_ReturnsFalse()
    {
        var id = Guid.NewGuid();
        _mockRepository.Setup(r => r.DeleteAsync(id)).ReturnsAsync(false);

        var result = await _service.DeleteTodoAsync(id);

        Assert.False(result);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task GetFilteredTodosAsync_WithCompletedFilter_ReturnsCompletedTodos()
    {
        var todos = new List<TodoItem>
        {
            new TodoItem("Todo 1"),
            new TodoItem("Todo 2")
        };
        todos[0].MarkAsCompleted();

        _mockRepository.Setup(r => r.GetFilteredAsync(true, null))
            .ReturnsAsync(new[] { todos[0] });

        var result = await _service.GetFilteredTodosAsync(isCompleted: true);

        Assert.Single(result);
        Assert.True(result.First().IsCompleted);
    }
}