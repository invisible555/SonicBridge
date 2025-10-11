using Backend.Entity;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repository
{
    public class TranscriptionRepository : ITranscriptionRepository
    {
        private readonly AppDbContext _context;

        public TranscriptionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddTaskAsync(TranscriptionTasks task)
        {
            _context.TranscriptionTasks.Add(task);
            await _context.SaveChangesAsync();
        }

        public async Task<TranscriptionTasks?> GetByTaskIdAsync(string taskId)
        {
            return await _context.TranscriptionTasks
                .FirstOrDefaultAsync(t => t.TaskId == taskId);
        }

        public async Task<TranscriptionTasks?> GetByUserFileAndLanguageAsync(int userId, string fileName, string language)
        {
            return await _context.TranscriptionTasks
                .FirstOrDefaultAsync(t => t.UserId == userId && t.FileName == fileName && t.Language == language);
        }

        public async Task UpdateStatusAsync(string taskId, string status, string? resultFile = null)
        {
            var task = await _context.TranscriptionTasks.FirstOrDefaultAsync(t => t.TaskId == taskId);
            if (task != null)
            {
                task.Status = status;
                task.ResultFile = resultFile;
                task.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateAsync(TranscriptionTasks task)
        {
            _context.TranscriptionTasks.Update(task);
            await _context.SaveChangesAsync();
        }

    }
}
