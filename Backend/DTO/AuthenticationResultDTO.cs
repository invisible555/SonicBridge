using Backend.Entity;

namespace Backend.DTO
{
    public class AuthenticationResultDTO
    {
        public bool Success { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpiryTime { get; set; }
        public string? Login { get; set; }
        public string? Role { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
