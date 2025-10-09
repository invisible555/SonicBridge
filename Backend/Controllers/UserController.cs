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
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;

        }
        [Authorize]
        [HttpGet("profile")]
            public IActionResult GetProfile()
            {
                var user = User.Identity?.Name;
                if (user == null)
                {
                    return Unauthorized("Nie jesteś zalogowany");
                }
                
                // Możesz zwrócić więcej informacji o użytkowniku, jeśli są dostępne
                return Ok(new { UserName = user });
        }
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Nie znaleziono użytkownika.");
            if (string.IsNullOrEmpty(request.CurrentPassword) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("Podaj aktualne i nowe hasło.");
            }
            var result = await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            if (!result)
            {
                return BadRequest("Nie udało się zmienić hasła. Sprawdź czy podałeś poprawne aktualne hasło.");
            }
            return Ok("Hasło zostało zmienione.");
        }
        [Authorize]
        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequestDTO request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Nie znaleziono użytkownika.");
            if (string.IsNullOrEmpty(request.NewEmail) || string.IsNullOrEmpty(request.Password))
            {
                return BadRequest("Podaj nowy email i hasło.");
            }
            var result = await _userService.ChangeEmailAsync(userId, request.NewEmail, request.Password);
            if (!result)
            {
                return BadRequest("Nie udało się zmienić emaila. Sprawdź czy podałeś poprawne hasło.");
            }
            return Ok("Email został zmieniony.");
        }
        [Authorize]
        [HttpPost("delete-account")]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized("Nie znaleziono użytkownika.");
            if (string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest("Podaj hasło.");
            }
            var result = await _userService.DeleteAccountAsync(userId, dto.Password);
            if (!result)
            {
                return BadRequest("Nie udało się usunąć konta. Sprawdź czy podałeś poprawne hasło.");
            }
            // Usuwanie ciasteczek z tokenami
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            return Ok("Konto zostało usunięte.");
        }
    }
   
}
