namespace TodoList.Application.DTOs;

public record TodoItemDto(
    Guid Id,
    string Title,
    string? Description,
    DateTime? DueDate,
    bool IsCompleted,
    DateTime CreatedAt,
    DateTime? UpdatedAt,
    bool IsOverdue
);

public record CreateTodoDto(
    string Title,
    string? Description = null,
    DateTime? DueDate = null
);

public record UpdateTodoDto(
    string? Title = null,
    string? Description = null,
    DateTime? DueDate = null
);

