using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.Dtos
{
    public class TaskItemDto
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime? DueDate { get; set; }
        public string AssignedTo { get; set; } = string.Empty;
    }
}
