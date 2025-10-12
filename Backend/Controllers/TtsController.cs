using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

[ApiController]
[Route("api/tts")]
public class TtsController : ControllerBase
{
    private readonly HttpClient _httpClient;
    private readonly string _pythonTtsUrl = "http://tts-api:8100/api/tts";

    public TtsController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private string GetUserId()
    {
        return User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value
            ?? "default";
    }

    // =======================================
    // 1️⃣  Generowanie TTS z obsługą cache
    // =======================================
    [Authorize]
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateTts(
        [FromForm] string text,
        [FromForm] string voice = "tts_models/en/ljspeech/tacotron2-DDC",
        [FromForm] string output_name = "output.wav"
    )
    {
        try
        {
            var userId = GetUserId();
            if (string.IsNullOrWhiteSpace(text))
                return BadRequest("Pole 'text' nie może być puste.");

            // 📁 Upewnij się, że katalog istnieje
            var userDir = Path.Combine("/shared/UserFiles", userId, "TTS");
            Directory.CreateDirectory(userDir);

            // 🧠 Stabilny hash z tekstu + modelu (deterministyczny)
            string GenerateStableHash(string input)
            {
                using var sha = SHA256.Create();
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").Substring(0, 8);
            }

            var safeName = Path.GetFileNameWithoutExtension(output_name);
            var hash = GenerateStableHash($"{text}_{voice}");
            var finalName = $"{safeName}_{hash}.wav";
            var filePath = Path.Combine(userDir, finalName);

            // 🔍 Sprawdź cache
            if (System.IO.File.Exists(filePath))
            {
                Console.WriteLine($"[TTS CACHE] ✅ Plik już istnieje: {filePath}");
                return Ok(new
                {
                    status = "done",
                    file_path = $"{userId}/TTS/{finalName}",
                    message = "Użyto istniejącego pliku z cache."
                });
            }

            Console.WriteLine($"[TTS GENERATE] ⏳ Tworzenie nowego pliku TTS dla user={userId}, model={voice}");

            // 📨 Przygotowanie danych do Pythona
            var form = new MultipartFormDataContent
            {
                { new StringContent(text), "text" },
                { new StringContent(voice), "voice" },
                { new StringContent(finalName), "output_name" },
                { new StringContent(userId), "user_id" }
            };

            // 📡 Wyślij do tts-api
            var response = await _httpClient.PostAsync($"{_pythonTtsUrl}/generate", form);
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[TTS ERROR] ❌ Python TTS zwrócił błąd {response.StatusCode}: {result}");
                return StatusCode((int)response.StatusCode, result);
            }

            Console.WriteLine($"[TTS OK] 🟢 Zadanie TTS utworzone dla user={userId}");
            return Content(result, "application/json");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TTS EXCEPTION] 💥 {ex}");
            return StatusCode(500, $"Błąd podczas generowania TTS: {ex.Message}");
        }
    }

    // ============================
    // 2️⃣  Sprawdzanie statusu
    // ============================
    [HttpGet("status/{taskId}")]
    public async Task<IActionResult> GetTtsStatus(string taskId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_pythonTtsUrl}/status/{taskId}");
            var result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return StatusCode((int)response.StatusCode, result);

            return Content(result, "application/json");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TTS STATUS ERROR] ⚠️ {ex}");
            return StatusCode(500, $"Błąd pobierania statusu TTS: {ex.Message}");
        }
    }

    // ============================
    // 3️⃣  Pobieranie pliku audio
    // ============================
    [HttpGet("download/{userId}/{fileName}")]
    public async Task<IActionResult> DownloadTtsFile(string userId, string fileName)
    {
        try
        {
            var response = await _httpClient.GetAsync($"{_pythonTtsUrl}/download/{userId}/{fileName}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"[TTS DOWNLOAD] ❌ Plik {fileName} nie został znaleziony (user={userId})");
                return NotFound($"Plik {fileName} nie został znaleziony dla użytkownika {userId}.");
            }

            var stream = await response.Content.ReadAsStreamAsync();
            Console.WriteLine($"[TTS DOWNLOAD] 📤 Wysłano plik {fileName} dla user={userId}");
            return File(stream, "audio/wav", fileName);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TTS DOWNLOAD ERROR] 💥 {ex}");
            return StatusCode(500, $"Błąd pobierania pliku TTS: {ex.Message}");
        }
    }
}
