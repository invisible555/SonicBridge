using Backend.DTO;
using Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranscriptionController : ControllerBase
    {
        private readonly ITranscriptionService _transcriptionService;

        public TranscriptionController(ITranscriptionService transcriptionService)
        {
            _transcriptionService = transcriptionService;
        }

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return null;
            return userId;
        }

        [Authorize]
        [HttpPost("get")]
        public async Task<IActionResult> GetOrGenerate([FromBody] TranscriptionRequestDTO request)
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized("Brak użytkownika.");

            if (string.IsNullOrEmpty(request.FileName))
                return BadRequest("Brak nazwy pliku.");

            try
            {
                var text = await _transcriptionService.GetOrGenerateTranscription(userId.Value, request.FileName, request.Language);
                return Ok(new { text });
            }
            catch (TimeoutException)
            {
                return StatusCode(504, "Czas transkrypcji został przekroczony.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Błąd serwera: {ex.Message}");
            }
        }
    }

 
}

