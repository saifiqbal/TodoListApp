using TodoList.Application.DTOs;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;
using TodoList.Application.Extensions;
namespace TodoList.Application.Services;

public class TodoService : ITodoService
{
    private readonly ITodoRepository _repository;

    public TodoService(ITodoRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<IEnumerable<TodoItemDto>> GetAllTodosAsync()
    {
        var items = await _repository.GetAllAsync();
        return items.Select(i => i.ToDto());
    }

    public async Task<TodoItemDto?> GetTodoByIdAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        return item?.ToDto();
    }

    public async Task<TodoItemDto> CreateTodoAsync(CreateTodoDto dto)
    {
        ValidateCreateDto(dto);

        var todoItem = new TodoItem(dto.Title, dto.Description, dto.DueDate);
        var created = await _repository.AddAsync(todoItem);
        await _repository.SaveChangesAsync();

        return created.ToDto();
    }

    public async Task<TodoItemDto> UpdateTodoAsync(Guid id, UpdateTodoDto dto)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null)
            throw new KeyNotFoundException($"Todo item with ID {id} not found");

        if (dto.Title != null)
            item.UpdateTitle(dto.Title);

        if (dto.Description != null)
            item.UpdateDescription(dto.Description);

        if (dto.DueDate.HasValue || dto.DueDate == null)
            item.UpdateDueDate(dto.DueDate);

        var updated = await _repository.UpdateAsync(item);
        await _repository.SaveChangesAsync();

        return updated.ToDto();
    }

    public async Task<TodoItemDto> MarkAsCompletedAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null)
            throw new KeyNotFoundException($"Todo item with ID {id} not found");

        item.MarkAsCompleted();
        var updated = await _repository.UpdateAsync(item);
        await _repository.SaveChangesAsync();

        return updated.ToDto();
    }

    public async Task<TodoItemDto> MarkAsIncompleteAsync(Guid id)
    {
        var item = await _repository.GetByIdAsync(id);
        if (item == null)
            throw new KeyNotFoundException($"Todo item with ID {id} not found");

        item.MarkAsIncomplete();
        var updated = await _repository.UpdateAsync(item);
        await _repository.SaveChangesAsync();

        return updated.ToDto();
    }

    public async Task<bool> DeleteTodoAsync(Guid id)
    {
        var result = await _repository.DeleteAsync(id);
        if (result)
            await _repository.SaveChangesAsync();
        
        return result;
    }

    public async Task<IEnumerable<TodoItemDto>> GetFilteredTodosAsync(bool? isCompleted = null, bool? isOverdue = null)
    {
        var items = await _repository.GetFilteredAsync(isCompleted, isOverdue);
        return items.Select(i => i.ToDto());
    }

    private void ValidateCreateDto(CreateTodoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Title is required", nameof(dto.Title));

        if (dto.Title.Length > 200)
            throw new ArgumentException("Title must be 200 characters or less", nameof(dto.Title));

        if (dto.Description?.Length > 2000)
            throw new ArgumentException("Description must be 2000 characters or less", nameof(dto.Description));
    }
}