using Backend.DTO;
using Backend.Entity;
using Backend.Repository;
using Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _userFilesPath = "/shared/UserFiles";
        public FileController(IFileService fileService, IWebHostEnvironment env, IHttpClientFactory httpClientFactory)
        {
            _fileService = fileService;
            _env = env;
            _httpClientFactory = httpClientFactory;
        }
        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Nie znaleziono użytkownika.");
            var uploadFolder = Path.Combine("/shared/UserFiles", userId.ToString());
            // var uploadFolder = "/shared/UserFiles";
            //var uploadFolder = Path.Combine(_env.ContentRootPath, "UserFiles");
            try
            {
                var userFile = await _fileService.UploadAsync(userId, file, uploadFolder);
                return Ok(new { message = "Plik zapisany", file = userFile });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> ListFiles()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Nie znaleziono użytkownika.");
            var files = await _fileService.GetFilesByUserIdAsync(userId);
            return Ok(files);
        }
        [Authorize]
        [HttpGet("download/{fileName}")]
        public async Task<IActionResult> GetAudioFile(string fileName)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Nie znaleziono użytkownika.");
            if (string.IsNullOrEmpty(fileName))
                return BadRequest("Nie podano nazwy pliku.");

            var uploadFolder = Path.Combine("/shared/UserFiles", userId.ToString());
            //var uploadFolder = "/shared/UserFiles";
            // var uploadFolder = Path.Combine(_env.ContentRootPath, "UserFiles");
            var filePath = Path.Combine(uploadFolder, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound("Plik nie istnieje.");

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            // Ustaw MIME typu audio, jeśli znasz rozszerzenie
            string contentType = "audio/mpeg";
            if (fileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                contentType = "audio/wav";

            return File(memory, contentType, fileName);
        }

        [Authorize]
        [HttpDelete("delete/{fileName}")]
        public async Task<IActionResult> DeleteFile(string fileName)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Nie znaleziono użytkownika.");

            if (string.IsNullOrEmpty(fileName))
                return BadRequest("Nie podano nazwy pliku.");

            var uploadFolder = Path.Combine("/shared/UserFiles", userId.ToString());
            var filePath = Path.Combine(uploadFolder, fileName);

            try
            {
                // 1️⃣ Usuń fizyczny plik
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // 2️⃣ Usuń z bazy danych
                await _fileService.DeleteFileAsync(userId, fileName);

                return Ok(new { message = "Plik został usunięty." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Błąd podczas usuwania pliku: {ex.Message}");
            }
        }

    }
}
