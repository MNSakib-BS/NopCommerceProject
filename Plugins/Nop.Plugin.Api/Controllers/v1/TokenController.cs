using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Api.Models.Authentication;
using Nop.Plugin.Api.Services;

namespace Nop.Plugin.Api.Controllers
{
    [AllowAnonymous]
    [Route("/token")]
    public class TokenController : Controller
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpGet]
        public async Task<IActionResult> Token(TokenRequest model)
        {
            var response = await _tokenService.GenerateAsync(model);
            return Json(response);
        }
    }
}
