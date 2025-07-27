using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
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
    }
}
