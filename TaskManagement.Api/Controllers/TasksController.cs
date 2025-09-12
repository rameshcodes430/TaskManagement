using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Api.Dtos;
using TaskManagement.Api.Services;

namespace TaskManagement.Api.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TasksController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TasksController> _logger;

        public TasksController(ITaskService taskService, ILogger<TasksController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItemDto>>> GetAllTasks()
        {
            var tasks = await _taskService.GetAllTasksAsync();
            return Ok(tasks);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItemDto>> GetTaskById(int id)
        {
            try
            {
                var task = await _taskService.GetTaskByIdAsync(id);
                return Ok(task);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Task with ID {TaskId} not found.", id);
                return NotFound(new { message = ex.Message });
            }
        }
        [HttpPost]
        public async Task<ActionResult<TaskItemDto>> CreateTask([FromBody] TaskItemDto taskItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var createdTask = await _taskService.CreateTaskAsync(taskItemDto);
            return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<TaskItemDto>> UpdateTask(int id, [FromBody] TaskItemDto taskItemDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var updatedTask = await _taskService.UpdateTaskAsync(id, taskItemDto);
                return Ok(updatedTask);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Task with ID {TaskId} not found for update.", id);
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            try
            {
                var result = await _taskService.DeleteTaskAsync(id);
                if (!result)
                {
                    return NotFound(new { message = $"Task with ID {id} not found." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting task with ID {TaskId}.", id);
                return StatusCode(500, new { message = "An error occurred while processing your request." });
            }
        }

    }
}
