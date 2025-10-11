using Backend.Entity;

namespace Backend.Repository
{
    public interface ITranscriptionRepository
    {
        Task AddTaskAsync(TranscriptionTasks task);
        Task<TranscriptionTasks?> GetByTaskIdAsync(string taskId);
        Task<TranscriptionTasks?> GetByUserFileAndLanguageAsync(int userId, string fileName, string language);
        Task UpdateStatusAsync(string taskId, string status, string? resultFile = null);
        Task UpdateAsync(TranscriptionTasks task);
    }
}
