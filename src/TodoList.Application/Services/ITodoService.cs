using TodoList.Application.DTOs;

public interface ITodoService
{
    Task<IEnumerable<TodoItemDto>> GetAllTodosAsync();
    Task<TodoItemDto?> GetTodoByIdAsync(Guid id);
    Task<TodoItemDto> CreateTodoAsync(CreateTodoDto dto);
    Task<TodoItemDto> UpdateTodoAsync(Guid id, UpdateTodoDto dto);
    Task<TodoItemDto> MarkAsCompletedAsync(Guid id);
    Task<TodoItemDto> MarkAsIncompleteAsync(Guid id);
    Task<bool> DeleteTodoAsync(Guid id);
    Task<IEnumerable<TodoItemDto>> GetFilteredTodosAsync(bool? isCompleted = null, bool? isOverdue = null);
}
