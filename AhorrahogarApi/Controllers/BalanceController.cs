using AhorrahogarApi.Constants;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Models;
using AhorrahogarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AhorrahogarApi.Controllers
{
    // Requiere sesión. El hogar sale de quien inició sesión, no de lo que mande la app.
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceService _service;
        private readonly IUsuarioActual _usuario;

        public BalanceController(IBalanceService service, IUsuarioActual usuario)
        {
            _service = service;
            _usuario = usuario;
        }

        private ObjectResult SinHogar() => StatusCode(403, "Primero únete a un hogar.");

        // Solo los balances de MI hogar
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BalanceDTO>>> GetAll()
        {
            var hogarId = await _usuario.HogarIdAsync();
            if (hogarId is null) return SinHogar();

            var todos = await _service.GetAllAsync();
            return Ok(todos.Where(b => b.hogar_id == hogarId));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BalanceDTO>> GetById(int id)
        {
            var hogarId = await _usuario.HogarIdAsync();
            if (hogarId is null) return SinHogar();

            var balance = await _service.GetByIdAsync(id);
            // Si es de otro hogar se responde igual que si no existiera
            return balance is null || balance.hogar_id != hogarId
                ? NotFound(HttpMessages.NotFound)
                : Ok(balance);
        }

        [HttpGet("{year:int}/{month:int}")]
        public async Task<ActionResult<BalanceDTO>> GetByPeriodo(int year, int month)
        {
            if (month is < 1 or > 12)
                return BadRequest(new Response<object>(false, HttpMessages.BadRequest));

            var hogarId = await _usuario.HogarIdAsync();
            if (hogarId is null) return SinHogar();

            var balance = await _service.GetByPeriodoAsync(hogarId.Value, year, month);

            // Un mes sin movimientos no es un error: se devuelve en ceros
            return Ok(balance ?? new BalanceDTO
            {
                hogar_id = hogarId.Value,
                periodo = new DateOnly(year, month, 1)
            });
        }

        [HttpGet("{anio:int}/{mes:int}/categorias")]
        public async Task<ActionResult<IEnumerable<CategoriaTotalDto>>> GetGastosPorCategoria(int anio, int mes)
        {
            if (mes < 1 || mes > 12 || anio < 1)
                return BadRequest("anio o mes fuera de rango");

            var hogarId = await _usuario.HogarIdAsync();
            if (hogarId is null) return SinHogar();

            return Ok(await _service.GetGastosPorCategoriaAsync(hogarId.Value, anio, mes));
        }
    }
}
