using AhorrahogarApi.Constants;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Models;
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
            return product is null ? NotFound(HttpMessages.NotFound) : Ok();
        }

        [HttpPost]
        public async Task<ActionResult<GoalsDto>> Create(CreateGoalsDto dto)
        {

            await _service.CreateAsync(dto);

            return StatusCode(201, HttpMessages.Confirm);

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Response<object>>> Update(int id, UpdateGoalsDto dto)
        {
            var updated = await _service.UpdateAsync(id, dto);

            return updated
                ? Ok(HttpMessages.Confirm)
                : NotFound(HttpMessages.NotFound);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Response<object>>> Delete(int id)
        {
            var delete = await _service.DeleteAsync(id);
            return delete
                 ? Ok(HttpMessages.Confirm)
                : NotFound(HttpMessages.NotFound);

        }
    }

}

