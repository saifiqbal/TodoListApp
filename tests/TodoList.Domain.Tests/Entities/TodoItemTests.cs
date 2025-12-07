using TodoList.Domain.Entities;
using Xunit;

namespace TodoList.Domain.Tests.Entities;

public class TodoItemTests
{
    [Fact]
    public void Constructor_WithValidTitle_CreatesTodoItem()
    {
        var title = "Test Todo";

        var todo = new TodoItem(title);

        Assert.NotEqual(Guid.Empty, todo.Id);
        Assert.Equal(title, todo.Title);
        Assert.Null(todo.Description);
        Assert.Null(todo.DueDate);
        Assert.False(todo.IsCompleted);
        Assert.True((DateTime.UtcNow - todo.CreatedAt).TotalSeconds < 1);
    }

    [Fact]
    public void Constructor_WithAllParameters_CreatesTodoItem()
    {
        var title = "Test Todo";
        var description = "Test Description";
        var dueDate = DateTime.UtcNow.AddDays(7);

        var todo = new TodoItem(title, description, dueDate);

        Assert.Equal(title, todo.Title);
        Assert.Equal(description, todo.Description);
        Assert.Equal(dueDate, todo.DueDate);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_WithInvalidTitle_ThrowsArgumentException(string invalidTitle)
    {
        Assert.Throws<ArgumentException>(() => new TodoItem(invalidTitle));
    }

    [Fact]
    public void UpdateTitle_WithValidTitle_UpdatesTitle()
    {
        var todo = new TodoItem("Original Title");
        var newTitle = "Updated Title";

        todo.UpdateTitle(newTitle);

        Assert.Equal(newTitle, todo.Title);
        Assert.NotNull(todo.UpdatedAt);
    }

    [Fact]
    public void UpdateTitle_WithEmptyTitle_ThrowsArgumentException()
    {
        var todo = new TodoItem("Original Title");

        Assert.Throws<ArgumentException>(() => todo.UpdateTitle(""));
    }

    [Fact]
    public void MarkAsCompleted_ChangesIsCompletedToTrue()
    {
        var todo = new TodoItem("Test Todo");

        todo.MarkAsCompleted();

        Assert.True(todo.IsCompleted);
        Assert.NotNull(todo.UpdatedAt);
    }

    [Fact]
    public void MarkAsIncomplete_ChangesIsCompletedToFalse()
    {
        var todo = new TodoItem("Test Todo");
        todo.MarkAsCompleted();

        todo.MarkAsIncomplete();

        Assert.False(todo.IsCompleted);
    }

    [Fact]
    public void IsOverdue_WithPastDueDateAndNotCompleted_ReturnsTrue()
    {
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var todo = new TodoItem("Test Todo", dueDate: pastDate);

        var isOverdue = todo.IsOverdue();

        Assert.True(isOverdue);
    }

    [Fact]
    public void IsOverdue_WithPastDueDateAndCompleted_ReturnsFalse()
    {
        var pastDate = DateTime.UtcNow.AddDays(-1);
        var todo = new TodoItem("Test Todo", dueDate: pastDate);
        todo.MarkAsCompleted();

        var isOverdue = todo.IsOverdue();

        Assert.False(isOverdue);
    }

    [Fact]
    public void IsOverdue_WithFutureDueDate_ReturnsFalse()
    {
        var futureDate = DateTime.UtcNow.AddDays(7);
        var todo = new TodoItem("Test Todo", dueDate: futureDate);

        var isOverdue = todo.IsOverdue();
        
        Assert.False(isOverdue);
    }

    [Fact]
    public void IsOverdue_WithNoDueDate_ReturnsFalse()
    {
        var todo = new TodoItem("Test Todo");

        var isOverdue = todo.IsOverdue();

        Assert.False(isOverdue);
    }
}