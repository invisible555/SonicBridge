using System.ComponentModel.DataAnnotations;

namespace Backend.DTO
{
    public class LoginRequestDTO
    {
        public string? Login { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
