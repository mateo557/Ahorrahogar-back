using AhorrahogarApi.Constants;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Models;
using AhorrahogarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AhorrahogarApi.Controllers
{
    
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class GoalController : ControllerBase
    {
        private readonly IGoalService _service;
        private readonly IUsuarioActual _usuario;

        public GoalController(IGoalService service, IUsuarioActual usuario)
        {
            _service = service;
            _usuario = usuario;
        }

        private ObjectResult SinHogar() => StatusCode(403, "Primero únete a un hogar.");

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GoalsDto>>> GetAll()
        {
            var hogarId = await _usuario.HogarIdAsync();
            if (hogarId is null) return SinHogar();

            return Ok(await _service.GetAllAsync(hogarId.Value));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GoalsDto>> GetById(int id)
        {
            var hogarId = await _usuario.HogarIdAsync();
            if (hogarId is null) return SinHogar();

            var meta = await _service.GetByIdAsync(id);
            // Si es de otro hogar se responde igual que si no existiera
            return meta is null || meta.Hogar_Id != hogarId
                ? NotFound(HttpMessages.NotFound)
                : Ok(meta);
        }

        [HttpPost]
        public async Task<ActionResult<GoalsDto>> Create(CreateGoalsDto dto)
        {
            var hogarId = await _usuario.HogarIdAsync();
            if (hogarId is null) return SinHogar();

            // El hogar lo pone el servidor, se ignora el que venga en el body
            dto.Hogar_Id = hogarId.Value;
            await _service.CreateAsync(dto);

            return StatusCode(201, HttpMessages.Confirm);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Response<object>>> Update(int id, UpdateGoalsDto dto)
        {
            var hogarId = await _usuario.HogarIdAsync();
            if (hogarId is null) return SinHogar();

            // Solo se puede editar una meta de MI hogar
            var existente = await _service.GetByIdAsync(id);
            if (existente is null || existente.Hogar_Id != hogarId)
                return NotFound(HttpMessages.NotFound);

            dto.Hogar_Id = hogarId.Value;
            var updated = await _service.UpdateAsync(id, dto);

            return updated
                ? Ok(HttpMessages.Confirm)
                : NotFound(HttpMessages.NotFound);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response<object>>> Delete(int id)
        {
            var hogarId = await _usuario.HogarIdAsync();
            if (hogarId is null) return SinHogar();

            var existente = await _service.GetByIdAsync(id);
            if (existente is null || existente.Hogar_Id != hogarId)
                return NotFound(HttpMessages.NotFound);

            var delete = await _service.DeleteAsync(id);
            return delete
                ? Ok(HttpMessages.Confirm)
                : NotFound(HttpMessages.NotFound);
        }
    }
}
