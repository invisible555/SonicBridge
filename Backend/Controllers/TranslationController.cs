using Backend.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TranslationController : ControllerBase
    {
        private readonly ITranslationService _translationService;

        public TranslationController(ITranslationService translationService)
        {
            _translationService = translationService;
        }
        [Authorize]
        [HttpPost("translation")]
        public async Task<IActionResult> Translate([FromForm] string text, [FromForm] string source_lang, [FromForm] string target_lang)
        {
            try
            {
                var result = await _translationService.TranslateAsync(text, source_lang, target_lang);
                return Content(result, "application/json");
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
