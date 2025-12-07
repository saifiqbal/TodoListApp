using TodoList.Application.DTOs;

namespace TodoList.Application.Extensions
{
    public static class TodoMappingExtensions
    {
        public static TodoItemDto ToDto(this Domain.Entities.TodoItem item)
        {
            return new TodoItemDto(
                item.Id,
                item.Title,
                item.Description,
                item.DueDate,
                item.IsCompleted,
                item.CreatedAt,
                item.UpdatedAt,
                item.IsOverdue()
            );
        }
    }
}
