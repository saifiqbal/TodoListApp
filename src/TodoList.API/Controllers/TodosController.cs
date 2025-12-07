
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.DTOs;
using TodoList.Application.Services;

namespace TodoList.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;
    private readonly ILogger<TodosController> _logger;

    public TodosController(ITodoService todoService, ILogger<TodosController> logger)
    {
        _todoService = todoService;
        _logger = logger;
    }

    /// <summary>
    /// Get all todo items
    /// </summary>
    /// <param name="isCompleted">Filter by completion status</param>
    /// <param name="isOverdue">Filter by overdue status</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TodoItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<TodoItemDto>>> GetAll(
        [FromQuery] bool? isCompleted = null,
        [FromQuery] bool? isOverdue = null)
    {
        _logger.LogInformation("Getting all todos. Completed: {IsCompleted}, Overdue: {IsOverdue}", 
            isCompleted, isOverdue);

        var todos = await _todoService.GetFilteredTodosAsync(isCompleted, isOverdue);
        return Ok(todos);
    }

    /// <summary>
    /// Get a specific todo item by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoItemDto>> GetById(Guid id)
    {
        _logger.LogInformation("Getting todo with ID: {Id}", id);

        var todo = await _todoService.GetTodoByIdAsync(id);
        if (todo == null)
        {
            _logger.LogWarning("Todo with ID {Id} not found", id);
            return NotFound(new { message = $"Todo item with ID {id} not found" });
        }

        return Ok(todo);
    }

    /// <summary>
    /// Create a new todo item
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoItemDto>> Create([FromBody] CreateTodoDto dto)
    {
        _logger.LogInformation("Creating new todo: {Title}", dto.Title);

        var created = await _todoService.CreateTodoAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Update an existing todo item
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoItemDto>> Update(Guid id, [FromBody] UpdateTodoDto dto)
    {
        _logger.LogInformation("Updating todo with ID: {Id}", id);

        try
        {
            var updated = await _todoService.UpdateTodoAsync(id, dto);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Todo with ID {Id} not found", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Mark a todo item as completed
    /// </summary>
    [HttpPatch("{id}/complete")]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoItemDto>> MarkComplete(Guid id)
    {
        _logger.LogInformation("Marking todo {Id} as completed", id);

        try
        {
            var updated = await _todoService.MarkAsCompletedAsync(id);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Todo with ID {Id} not found", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Mark a todo item as incomplete
    /// </summary>
    [HttpPatch("{id}/incomplete")]
    [ProducesResponseType(typeof(TodoItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoItemDto>> MarkIncomplete(Guid id)
    {
        _logger.LogInformation("Marking todo {Id} as incomplete", id);

        try
        {
            var updated = await _todoService.MarkAsIncompleteAsync(id);
            return Ok(updated);
        }
        catch (KeyNotFoundException ex)
        {
            _logger.LogWarning(ex, "Todo with ID {Id} not found", id);
            return NotFound(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a todo item
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Deleting todo with ID: {Id}", id);

        var deleted = await _todoService.DeleteTodoAsync(id);
        if (!deleted)
        {
            _logger.LogWarning("Todo with ID {Id} not found", id);
            return NotFound(new { message = $"Todo item with ID {id} not found" });
        }

        return NoContent();
    }
}