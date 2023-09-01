using Application.Interfaces;
using Domain.Models;
using Infrastructure.Caching;
using Microsoft.AspNetCore.Mvc;

namespace WebApiCleanArchitecture.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ICachingService _cachingService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            var authResponse = await _authService.AuthenticateAsync(model);
            return Ok(authResponse);
        }

        //[HttpPost("refresh")]
        //public async IActionResult RefreshToken(RefreshTokenRequestModel model)
        //{
        //    var cachedRefreshToken = await _cachingService.GetAsync(model. ToString());
        //    if (cachedRefreshToken != model.RefreshToken)
        //    {
        //        throw new ApplicationException("Invalid refresh token.");
        //    }

        //    // Lanjutkan dengan pembuatan token baru

        //    return Ok(newTokenResponse);
        //}
    }
}
