using Backend.Entity;

namespace Backend.DTO
{
    public class RegistrationResultDTO
    {
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
