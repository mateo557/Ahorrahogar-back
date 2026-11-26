using System.Security.Claims;
using AhorrahogarApi.Constants;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AhorrahogarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service) => _service = service;

        [HttpPost("register")]
        [EnableRateLimiting("auth")]
        public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto dto)
        {
            var resultado = await _service.RegisterAsync(dto);

            return resultado is null
                ? Conflict("Ese correo ya está registrado.")
                : StatusCode(201, resultado);
        }

        [HttpPost("login")]
        [EnableRateLimiting("auth")]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
        {
            var resultado = await _service.LoginAsync(dto);

            return resultado is null
                ? Unauthorized("Correo o contraseña incorrectos.")
                : Ok(resultado);
        }

        // Quién soy y si ya tengo hogar: la app lo usa al abrir para decidir qué pantalla mostrar
        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<MeDto>> Me()
        {
            var idTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idTexto, out var id))
                return Unauthorized(HttpMessages.Unauthorized);

            var me = await _service.GetMeAsync(id);

            return me is null
                ? Unauthorized(HttpMessages.Unauthorized)
                : Ok(me);
        }

        // Cambiar la contraseña estando con sesión iniciada. Pide la actual.
        // Los errores son 400 (no 401) porque la app interpreta 401 como "sesión vencida".
        [HttpPost("cambiar-password")]
        [Authorize]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> CambiarPassword(CambiarPasswordDto dto)
        {
            var idTexto = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(idTexto, out var id))
                return Unauthorized(HttpMessages.Unauthorized);

            return await _service.CambiarPasswordAsync(id, dto) switch
            {
                CambioPassword.Ok => NoContent(),
                CambioPassword.ActualIncorrecta => BadRequest("La contraseña actual no es correcta."),
                CambioPassword.IgualALaActual => BadRequest("La nueva contraseña debe ser distinta a la actual."),
                _ => Unauthorized(HttpMessages.Unauthorized)
            };
        }
    }
}
