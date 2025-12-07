using TodoList.Domain.Entities;

namespace TodoList.Domain.Interfaces;

public interface ITodoRepository
{
    Task<IEnumerable<TodoItem>> GetAllAsync();
    Task<TodoItem?> GetByIdAsync(Guid id);
    Task<TodoItem> AddAsync(TodoItem item);
    Task<TodoItem> UpdateAsync(TodoItem item);
    Task<bool> DeleteAsync(Guid id);
    Task<IEnumerable<TodoItem>> GetFilteredAsync(bool? isCompleted = null, bool? isOverdue = null);
    Task SaveChangesAsync();
}