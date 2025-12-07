using System.Text.Json;
using TodoList.Domain.Entities;
using TodoList.Domain.Interfaces;

namespace TodoList.Infrastructure.Persistence;

public class JsonFileRepository : ITodoRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private List<TodoItem> _cache;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public JsonFileRepository(string? filePath = null)
    {
        _filePath = filePath ?? Path.Combine(AppContext.BaseDirectory, "todos.json");
        _cache = LoadFromFile();
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            return _cache.ToList();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<TodoItem?> GetByIdAsync(Guid id)
    {
        await _semaphore.WaitAsync();
        try
        {
            return _cache.FirstOrDefault(t => t.Id == id);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<TodoItem> AddAsync(TodoItem item)
    {
        await _semaphore.WaitAsync();
        try
        {
            _cache.Add(item);
            return item;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<TodoItem> UpdateAsync(TodoItem item)
    {
        await _semaphore.WaitAsync();
        try
        {
            var index = _cache.FindIndex(t => t.Id == item.Id);
            if (index == -1)
                throw new KeyNotFoundException($"Todo item with ID {item.Id} not found");

            _cache[index] = item;
            return item;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        await _semaphore.WaitAsync();
        try
        {
            var item = _cache.FirstOrDefault(t => t.Id == id);
            if (item == null)
                return false;

            _cache.Remove(item);
            return true;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<IEnumerable<TodoItem>> GetFilteredAsync(bool? isCompleted = null, bool? isOverdue = null)
    {
        await _semaphore.WaitAsync();
        try
        {
            var query = _cache.AsEnumerable();

            if (isCompleted.HasValue)
                query = query.Where(t => t.IsCompleted == isCompleted.Value);

            if (isOverdue.HasValue && isOverdue.Value)
                query = query.Where(t => t.IsOverdue());

            return query.ToList();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task SaveChangesAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            var directory = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var json = JsonSerializer.Serialize(_cache, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private List<TodoItem> LoadFromFile()
    {
        if (!File.Exists(_filePath))
            return new List<TodoItem>();

        try
        {
            var json = File.ReadAllText(_filePath);
            return JsonSerializer.Deserialize<List<TodoItem>>(json, _jsonOptions) 
                   ?? new List<TodoItem>();
        }
        catch (Exception)
        {
            return new List<TodoItem>();
        }
    }
}