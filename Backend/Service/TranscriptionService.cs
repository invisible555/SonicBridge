using Microsoft.AspNetCore.Mvc;
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

            // ✅ 1. Jeśli plik istnieje – zwróć od razu treść
            if (System.IO.File.Exists(transcriptionFile))
            {
                return await System.IO.File.ReadAllTextAsync(transcriptionFile);
            }

            // ✅ 2. Wyślij tylko zlecenie do Whisper API, nie czekaj na wynik
            var client = _httpClientFactory.CreateClient("WhisperApi");

            using var form = new MultipartFormDataContent();
            form.Add(new StringContent(fileName), "file_name");
            form.Add(new StringContent(lang), "language");
            form.Add(new StringContent(userId.ToString()), "user_id");

            var response = await client.PostAsync("/transcribe", form);
            response.EnsureSuccessStatusCode();

            // ✅ Zwracamy komunikat że transkrypcja w toku
            return "Transkrypcja rozpoczęta. Sprawdź później.";
        }
        
      
    }
}
