using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

    // ============================
    // 1️⃣  Wysłanie tekstu do Pythona
    // ============================
    [HttpPost("generate")]
    public async Task<IActionResult> GenerateTts([FromForm] string text, [FromForm] string voice = "tts_models/en/ljspeech/tacotron2-DDC")
    {
        var form = new MultipartFormDataContent();
        form.Add(new StringContent(text), "text");
        form.Add(new StringContent(voice), "voice");
        form.Add(new StringContent("output.wav"), "output_name");

        var response = await _httpClient.PostAsync($"{_pythonTtsUrl}/generate", form);
        var result = await response.Content.ReadAsStringAsync();

        return Content(result, "application/json");
    }

    // ============================
    // 2️⃣  Sprawdzanie statusu
    // ============================
    [HttpGet("status/{taskId}")]
    public async Task<IActionResult> GetTtsStatus(string taskId)
    {
        var response = await _httpClient.GetAsync($"{_pythonTtsUrl}/status/{taskId}");
        var result = await response.Content.ReadAsStringAsync();

        return Content(result, "application/json");
    }

    // ============================
    // 3️⃣  Pobieranie pliku audio
    // ============================
    [HttpGet("download/{fileName}")]
    public async Task<IActionResult> DownloadTtsFile(string fileName)
    {
        var response = await _httpClient.GetAsync($"{_pythonTtsUrl}/download/{fileName}");

        if (!response.IsSuccessStatusCode)
            return NotFound();

        var stream = await response.Content.ReadAsStreamAsync();
        return File(stream, "audio/wav", fileName);
    }
}
