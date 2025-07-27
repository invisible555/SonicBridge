using Backend.DTO;
using Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _env;

        public AuthController(IAuthService userService, IWebHostEnvironment env)
        {
            _authService = userService;
            _env = env;

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrEmpty(user.Login))
            {
                return BadRequest("Podaj login lub email");
            }
            var _user = await _authService.Authenticate(user.Login, user.Password);
            if (_user.Success == false)
            {
                return Unauthorized("Nieprawidłowy login lub hasło");
            }
            if (_user.AccessToken !=null && _user.RefreshToken != null)
            {
                Response.Cookies.Append("access_token", _user.AccessToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // tylko HTTPS w produkcji!
                    SameSite = SameSiteMode.Strict, // lub Lax, zależnie od potrzeb
                    Expires = _user.ExpiryTime
                   
                });

                // (opcjonalnie) refresh token w ciasteczku
                Response.Cookies.Append("refresh_token", _user.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7) // np. dłużej niż access token
                });
            }
            return Ok(new
            {
                
                login = user.Login,
                role = _user.Role,
                tokenExpiredTime = _user.ExpiryTime,
            });
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] RegistrationRequestDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (string.IsNullOrEmpty(user.Login) || string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.Password))
            {
                return BadRequest("Login, email i hasło są wymagane.");
            }
            var result = await _authService.RegisterUser(user);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
            
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            // Usuwanie ciasteczek z tokenami
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("Nie znaleziono użytkownika");
            }

            
            var refreshToken = Request.Cookies["refresh_token"];
            var accessToken = Request.Cookies["access_token"];
            if(string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(accessToken))
            {
                return BadRequest("Brak tokenów do wylogowania.");
            }
            await _authService.LogoutAsync(userId, refreshToken,accessToken);


            return Ok(new { message = "Zostałeś wylogowany." });
        }
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out var userId))
            {
                return Unauthorized("Nie znaleziono użytkownika");
            }
            var userLogin = User.FindFirst(ClaimTypes.Name)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            return Ok(new
            {
                UserId = userId,
                Login = userLogin,
                Role = userRole
            });
        }
        [HttpPost("refresh-access-token")]
        public async Task<IActionResult> RefreshAccessToken()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized("Brak refresh tokena w ciasteczku.");

            var result = await _authService.RefreshAccessTokenAsync(refreshToken);

            if (!result.Success)
                return Unauthorized(result.ErrorMessage);

            // Nowy access token do cookie
            Response.Cookies.Append("access_token", result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = result.ExpiryTime
            });

            // Nowy refresh token (opcjonalnie)
            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                Response.Cookies.Append("refresh_token", result.RefreshToken, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddDays(7) // lub dłużej
                });
            }

            return Ok(new
            {
                message = "Token odświeżony",
                tokenExpiredTime = result.ExpiryTime
                // nie zwracaj tokenów do frontu!
            });
        }

    }
}
