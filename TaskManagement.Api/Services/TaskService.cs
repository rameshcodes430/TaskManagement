using Microsoft.EntityFrameworkCore;
using TaskManagement.Api.Data;
using TaskManagement.Api.Dtos;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Services
{
    public class TaskService : ITaskService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TaskService> _logger;

        public TaskService(AppDbContext context, ILogger<TaskService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasksAsync()
        {
            _logger.LogInformation("Fetching all tasks from the database.");
            return await _context.TaskItems.ToListAsync();
        }

        public async Task<TaskItem?> GetTaskByIdAsync(int id)
        {
            _logger.LogInformation("Fetching task with ID {TaskId} from the database.", id);
            var task = await _context.TaskItems.FindAsync(id);
            if (task == null)
            {
                _logger.LogWarning("Task with ID {TaskId} not found.", id);
                throw new KeyNotFoundException($"Task with ID {id} not found.");
            }
            return task;
        }

        public async Task<TaskItem> CreateTaskAsync(TaskItemDto taskItemdto)
        {
            _logger.LogInformation("Creating a new task.");
            var taskItem = new TaskItem
            {
                Title = taskItemdto.Title,
                Description = taskItemdto.Description,
                IsCompleted = taskItemdto.IsCompleted,
                DueDate = taskItemdto.DueDate,
                AssignedTo = taskItemdto.AssignedTo
            };

            _context.TaskItems.Add(taskItem);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Task created with ID {TaskId}.", taskItem.Id);
            return taskItem;
        }

        public async Task<TaskItem> UpdateTaskAsync(int id, TaskItemDto taskItemDto)
        {
            _logger.LogInformation("Updating task with ID {TaskId}.", id);
            var existingTask = await GetTaskByIdAsync(id);
            if (existingTask == null)
            {
                _logger.LogWarning("Task with ID {TaskId} not found for update.", id);
                throw new KeyNotFoundException($"Task with ID {id} not found.");
            }
            existingTask.Title = taskItemDto.Title;
            existingTask.Description = taskItemDto.Description;
            existingTask.IsCompleted = taskItemDto.IsCompleted;
            existingTask.DueDate = taskItemDto.DueDate;
            existingTask.AssignedTo = taskItemDto.AssignedTo;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Task with ID {TaskId} updated successfully.", id);
            return existingTask;
        }

        public async Task<bool> DeleteTaskAsync(int id)
        {
            _logger.LogInformation("Deleting task with ID {TaskId}.", id);
            var existingTask = await GetTaskByIdAsync(id);
            if (existingTask == null)
            {
                _logger.LogWarning("Task with ID {TaskId} not found for deletion.", id);
                throw new KeyNotFoundException($"Task with ID {id} not found.");
            }
            _context.TaskItems.Remove(existingTask);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Task with ID {TaskId} deleted successfully.", id);
            return true;
        }
    }
}
