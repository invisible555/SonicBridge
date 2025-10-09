using Backend.Entity;
using Backend.Repository;
using Microsoft.EntityFrameworkCore;

namespace Backend.Service
{
    public class TranscriptionService : ITranscriptionService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly TranscriptionRepository _repository;
        private readonly string _baseUserFilesPath = "/shared/UserFiles";

        public TranscriptionService(IHttpClientFactory httpClientFactory, TranscriptionRepository repository)
        {
            _httpClientFactory = httpClientFactory;
            _repository = repository;
        }

        public async Task<object> GetOrGenerateTranscription(int userId, string fileName, string language)
        {
            string lang = string.IsNullOrEmpty(language) ? "pl" : language;

            // 🔎 1. Sprawdź czy task już istnieje w repo
            var existingTask = await _repository.GetByUserFileAndLanguageAsync(userId, fileName, lang);

            if (existingTask != null)
            {
                // ✅ Jeśli transkrypcja skończona -> zwróć plik
                if (!string.IsNullOrEmpty(existingTask.ResultFile) &&
                    System.IO.File.Exists(existingTask.ResultFile))
                {
                    string text = await System.IO.File.ReadAllTextAsync(existingTask.ResultFile);
                    return new { status = "done", text, taskId = existingTask.TaskId };
                }

                // 🔄 Jeśli w trakcie -> zwróć status
                return new { status = existingTask.Status, taskId = existingTask.TaskId };
            }

            // 🚀 2. Jeśli nie ma taska → wyślij żądanie do Whisper API
            var client = _httpClientFactory.CreateClient("WhisperApi");

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(fileName), "file_name");
            form.Add(new StringContent(lang), "language");
            form.Add(new StringContent(userId.ToString()), "user_id");

            var response = await client.PostAsync("/transcribe", form);
            response.EnsureSuccessStatusCode();

            var taskId = Guid.NewGuid().ToString(); // Można nadpisać taskId odpowiedzią z Celery

            // 📝 Zapisz do repo
            var newTask = new TranscriptionTasks
            {
                UserId = userId,
                FileName = fileName,
                Language = lang,
                TaskId = taskId,
                Status = "pending",
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddTaskAsync(newTask);

            return new { status = "pending", taskId };
        }
    }
}
