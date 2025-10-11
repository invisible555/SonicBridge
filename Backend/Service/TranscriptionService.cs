using Backend.Entity;
using Backend.Repository;
using Microsoft.EntityFrameworkCore;
using System.Net.Http;
using System.Net.Http.Json;

namespace Backend.Service
{
    public sealed class WhisperQueueResponse
    {
        public string? task_id { get; set; }
        public string? status { get; set; }
    }

    public sealed class WhisperStatusResult
    {
        public string? file_path { get; set; }
        public string? language { get; set; }
    }

    public sealed class WhisperStatusResponse
    {
        public string? task_id { get; set; }
        public string? status { get; set; } // PENDING / STARTED / SUCCESS / FAILURE
        public WhisperStatusResult? result { get; set; }
        public string? error { get; set; }
    }

    public class TranscriptionService : ITranscriptionService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITranscriptionRepository _repository;
        private readonly string _baseUserFilesPath = "/shared/UserFiles";

        public TranscriptionService(IHttpClientFactory httpClientFactory, ITranscriptionRepository repository)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<object> GetOrGenerateTranscription(int userId, string fileName, string language)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name is required.", nameof(fileName));

            string lang = string.IsNullOrWhiteSpace(language) ? "pl" : language;

            var client = _httpClientFactory.CreateClient("WhisperApi");
            if (client == null)
                throw new InvalidOperationException("HttpClient 'WhisperApi' is not configured.");

            // 1️⃣ Spróbuj znaleźć istniejące zadanie
            var existingTask = await _repository.GetByUserFileAndLanguageAsync(userId, fileName, lang);

            if (existingTask != null)
            {
                // 1a) Jeśli mamy wynik i plik istnieje → zwróć tekst
                if (!string.IsNullOrEmpty(existingTask.ResultFile) && File.Exists(existingTask.ResultFile))
                {
                    var text = await File.ReadAllTextAsync(existingTask.ResultFile);
                    return new { status = "done", text, taskId = existingTask.TaskId };
                }

                // 1b) Jeśli mamy TaskId → sprawdź status
                if (!string.IsNullOrEmpty(existingTask.TaskId))
                {
                    WhisperStatusResponse? statusResp = null;

                    try
                    {
                        statusResp = await client.GetFromJsonAsync<WhisperStatusResponse>($"/status/{existingTask.TaskId}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"⚠️ [WhisperAPI] Nie udało się pobrać statusu: {ex.Message}");
                    }

                    if (statusResp == null)
                        return new { status = "pending", taskId = existingTask.TaskId };

                    // ✅ SUCCESS
                    if (string.Equals(statusResp.status, "SUCCESS", StringComparison.OrdinalIgnoreCase)
                        && statusResp.result?.file_path is string readyPath
                        && File.Exists(readyPath))
                    {
                        existingTask.Status = "done";
                        existingTask.ResultFile = readyPath;
                        existingTask.UpdatedAt = DateTime.UtcNow;
                        await _repository.UpdateAsync(existingTask);

                        var text = await File.ReadAllTextAsync(readyPath);
                        return new { status = "done", text, taskId = existingTask.TaskId };
                    }

                    // ❌ FAILURE
                    if (string.Equals(statusResp.status, "FAILURE", StringComparison.OrdinalIgnoreCase))
                    {
                        existingTask.Status = "error";
                        existingTask.UpdatedAt = DateTime.UtcNow;
                        await _repository.UpdateAsync(existingTask);

                        return new
                        {
                            status = "error",
                            taskId = existingTask.TaskId,
                            error = statusResp.error ?? "Transkrypcja nie powiodła się."
                        };
                    }

                    // ⏳ Nadal w toku
                    return new { status = "pending", taskId = existingTask.TaskId };
                }

                // Jeśli brak TaskId → utwórz nowe zadanie
                Console.WriteLine($"⚠️ Istniejący wpis bez TaskId dla pliku: {fileName}");
            }

            // 2️⃣ Brak zadania — utwórz nowe
            using var form = new MultipartFormDataContent
            {
                { new StringContent(fileName), "file_name" },
                { new StringContent(lang), "language" },
                { new StringContent(userId.ToString()), "user_id" }
            };

            HttpResponseMessage enqueueResponse;
            try
            {
                enqueueResponse = await client.PostAsync("/transcribe", form);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [WhisperAPI] Błąd podczas POST /transcribe: {ex.Message}");
                throw new InvalidOperationException("Nie udało się połączyć z serwerem transkrypcji (FastAPI).", ex);
            }

            if (!enqueueResponse.IsSuccessStatusCode)
            {
                var errBody = await enqueueResponse.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Whisper API zwrócił błąd {enqueueResponse.StatusCode}: {errBody}");
            }

            var queue = await enqueueResponse.Content.ReadFromJsonAsync<WhisperQueueResponse>();
            var taskId = queue?.task_id ?? Guid.NewGuid().ToString();

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

            Console.WriteLine($"✅ Utworzono nowe zadanie transkrypcji: {taskId} dla pliku {fileName}");

            return new { status = "pending", taskId };
        }
    }
}
