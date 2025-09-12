using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }
        public string AssignedTo { get; set; } = string.Empty;
    }
}
