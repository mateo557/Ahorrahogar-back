using AhorrahogarApi.DTOs;
using AhorrahogarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AhorrahogarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoalController : ControllerBase
    {
        private readonly IGoalService _service;

        public GoalController(IGoalService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GoalsDto>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<GoalsDto>> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            return product is null ? NotFound() : Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<GoalsDto>> Create(CreateGoalsDto dto)
        {

            var created = await _service.CreateAsync(dto);

            // Retorna HTTP 200 OK con el GetBalanceDTO resultante
            return Ok(dto);

            //try
            //{


            //}
            //catch (ArgumentException ex)
            //{
            //    // Si el monto es <= 0, retorna HTTP 400 Bad Request con el mensaje de error
            //    return BadRequest(new { message = ex.Message });
            //}
            //catch (System.Exception ex)
            //{
            //    // Para cualquier otro error inesperado, retorna HTTP 500
            //    return StatusCode(500, new { message = "Error interno del servidor", detail = ex.Message });
            //}
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UpdateGoalsDto dto) =>
            await _service.UpdateAsync(id, dto) ? NoContent() : NotFound();

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }

}

