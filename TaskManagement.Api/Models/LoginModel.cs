using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Api.Models
{
    public class LoginModel
    {
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Username { get; set; } = string.Empty;
        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;
    }
}
