namespace Backend.DTO
{
    public class TranscriptionStatusUpdateDTO
    {
        public int UserId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Language { get; set; } = "pl";
        public string Status { get; set; } = "pending";
        public string? ResultFile { get; set; }
        public string? Error { get; set; }
    }
}