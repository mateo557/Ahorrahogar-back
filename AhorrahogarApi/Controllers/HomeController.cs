using System.Security.Claims;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AhorrahogarApi.Controllers
{
    // Todo requiere sesión iniciada. El usuario sale del token, nunca de lo que mande la app.
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IHogarService _service;

        public HomeController(IHogarService service) => _service = service;

        private int UsuarioId =>
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 0;

        // Si salió bien devuelve los datos; si no, el mensaje de error con su código HTTP
        private IActionResult Responder<T>(HogarResultado<T> r) =>
            r.Ok ? StatusCode(r.Status, r.Datos) : StatusCode(r.Status, r.Mensaje);

        [HttpPost]
        public async Task<IActionResult> Crear(CrearHogarDto dto) =>
            Responder(await _service.CrearAsync(UsuarioId, dto.nombre));

        [HttpGet("mi-hogar")]
        public async Task<IActionResult> MiHogar() =>
            Responder(await _service.GetMiHogarAsync(UsuarioId));

        // Limitado por IP para que nadie pruebe códigos al azar
        [HttpPost("unirse")]
        [EnableRateLimiting("auth")]
        public async Task<IActionResult> Unirse(UnirseDto dto) =>
            Responder(await _service.UnirseAsync(UsuarioId, dto.codigo));

        [HttpDelete("mi-solicitud")]
        public async Task<IActionResult> CancelarSolicitud() =>
            Responder(await _service.CancelarSolicitudAsync(UsuarioId));

        [HttpGet("solicitudes")]
        public async Task<IActionResult> Solicitudes() =>
            Responder(await _service.GetSolicitudesAsync(UsuarioId));

        [HttpPost("solicitudes/{id:int}/aceptar")]
        public async Task<IActionResult> Aceptar(int id) =>
            Responder(await _service.ResolverSolicitudAsync(UsuarioId, id, true));

        [HttpPost("solicitudes/{id:int}/rechazar")]
        public async Task<IActionResult> Rechazar(int id) =>
            Responder(await _service.ResolverSolicitudAsync(UsuarioId, id, false));

        [HttpPost("codigo/regenerar")]
        public async Task<IActionResult> RegenerarCodigo() =>
            Responder(await _service.RegenerarCodigoAsync(UsuarioId));

        [HttpDelete("miembros/{id:int}")]
        public async Task<IActionResult> RemoverMiembro(int id) =>
            Responder(await _service.RemoverMiembroAsync(UsuarioId, id));

        [HttpPost("salir")]
        public async Task<IActionResult> Salir() =>
            Responder(await _service.SalirAsync(UsuarioId));
    }
}
