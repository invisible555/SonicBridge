using System.Net.Http;
using System.Text.Json;

namespace Backend.Service
{
    public class TranscriptionService : ITranscriptionService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _baseUserFilesPath = "/shared/UserFiles";

        public TranscriptionService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> GetOrGenerateTranscription(int userId, string fileName, string language)
        {
            string lang = string.IsNullOrEmpty(language) ? "pl" : language;

            var userFolder = Path.Combine(_baseUserFilesPath, userId.ToString());
            if (!Directory.Exists(userFolder))
                Directory.CreateDirectory(userFolder);

            string baseName = Path.GetFileNameWithoutExtension(fileName);
            string transcriptionFile = Path.Combine(userFolder, $"{baseName}_original_{lang}.txt");

            // 🔹 1. Cache
            if (System.IO.File.Exists(transcriptionFile))
            {
                return await System.IO.File.ReadAllTextAsync(transcriptionFile);
            }

            // 🔹 2. Whisper API
            var client = _httpClientFactory.CreateClient("WhisperApi");

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(fileName), "file_name");
            form.Add(new StringContent(lang), "language");

            var response = await client.PostAsync("/transcribe", form);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var parsed = JsonDocument.Parse(json);
            var taskId = parsed.RootElement.GetProperty("task_id").GetString();

            string? resultText = null;
            for (int i = 0; i < 30; i++)
            {
                await Task.Delay(2000);
                var statusResp = await client.GetAsync($"/status/{taskId}");
                var statusJson = await statusResp.Content.ReadAsStringAsync();
                var statusDoc = JsonDocument.Parse(statusJson);
                var status = statusDoc.RootElement.GetProperty("status").GetString();

                if (status == "done")
                {
                    resultText = statusDoc.RootElement.GetProperty("result").GetProperty("text").GetString();
                    break;
                }
                if (status == "error")
                    throw new Exception("Błąd transkrypcji w Whisper API");
            }

            if (resultText == null)
                throw new TimeoutException("Przekroczono czas oczekiwania na transkrypcję.");

            // 🔹 3. Zapisz cache
            await System.IO.File.WriteAllTextAsync(transcriptionFile, resultText);

            return resultText;
        }
    }
}
