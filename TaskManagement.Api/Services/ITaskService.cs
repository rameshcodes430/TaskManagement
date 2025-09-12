using TaskManagement.Api.Dtos;
using TaskManagement.Api.Models;

namespace TaskManagement.Api.Services
{
    public interface ITaskService
    {
        Task<IEnumerable<TaskItem>> GetAllTasksAsync();
        Task<TaskItem?> GetTaskByIdAsync(int id);
        Task<TaskItem> CreateTaskAsync(TaskItemDto taskItemDto);
        Task<TaskItem> UpdateTaskAsync(int id, TaskItemDto taskItemDto);
        Task<bool> DeleteTaskAsync(int id);
    }
}
