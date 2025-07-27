using System.ComponentModel.DataAnnotations;

namespace Backend.Entity
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Login jest wymagany.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Login musi mieć od 3 do 50 znaków.")]
        public string Login { get; set; } = null!;

        [Required(ErrorMessage = "Hasło jest wymagane.")]
        [MinLength(6, ErrorMessage = "Hasło musi mieć co najmniej 6 znaków.")]
        [RegularExpression(@"^(?=.*[0-9])(?=.*[\W_]).+$", ErrorMessage = "Hasło musi zawierać co najmniej jedną cyfrę i jeden znak specjalny.")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Nieprawidłowy format adresu e-mail.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Rola użytkownika jest wymagana.")]
        [RegularExpression("^(User|Admin)$", ErrorMessage = "Dozwolone role to: User lub Admin.")]
        public string Role { get; set; } = "User";

        // Nawigacja do plików użytkownika
        public ICollection<UserFile> Files { get; set; } = new List<UserFile>();
    }
}
