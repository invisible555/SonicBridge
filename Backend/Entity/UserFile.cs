namespace Backend.Entity
{
    public class UserFile
    {
        public int Id { get; set; }

        // RELACJA z użytkownikiem (wiele plików należy do jednego użytkownika)
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
    }
}
