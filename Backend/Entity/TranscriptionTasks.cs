using System.ComponentModel.DataAnnotations;

namespace Backend.Entity
{
    public class TranscriptionTasks
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string FileName { get; set; } = "";
        public string Language { get; set; } = "";
        public string TaskId { get; set; } = "";
        public string Status { get; set; } = "pending"; // pending, started, done, error
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // ✅ Nowe pole
        public string? ResultFile { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
