using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Sales.Api.utils;
using Sales.Application.Dtos;
using Sales.Application.Interfaces;
using System.Security.Claims;

namespace Sales.Api.Controllers
{
    [ApiController]
    [Route("/api/v1/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(ApiResponse<LoginResponseDto>.Failure("Usuario y contraseña son obligatorios.", statusCode: 400));
            }

            var result = await _authService.AuthenticateAsync(request.Username, request.Password);

            if (!result.IsSuccess)
            {
                return Unauthorized(ApiResponse<LoginResponseDto>.Failure(result.Message, statusCode: 401));
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, result.Username!),
                new Claim(ClaimTypes.Role, "User")
            };

            var claimsIdentity = new ClaimsIdentity(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });

            return Ok(ApiResponse<LoginResponseDto>.SuccessFul(data: result, message: "Login exitoso."));
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<string>>> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok(ApiResponse<string>.SuccessFul(data: "OK", message: "Sesión cerrada."));
        }

        [HttpGet("me")]
        public ActionResult<ApiResponse<string>> Me()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Unauthorized(ApiResponse<string>.Failure("No autenticado.", statusCode: 401));
            }

            return Ok(ApiResponse<string>.SuccessFul(
                data: User.Identity!.Name!,
                message: "Autenticado."));
        }
    }
}
