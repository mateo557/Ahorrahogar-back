using AhorrahogarApi.Constants;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AhorrahogarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceService _service;

        public BalanceController(IBalanceService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BalanceDTO>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{hogarId}")]
        public async Task<ActionResult<BalanceDTO>> GetById(int hogarId)
        {
            var product = await _service.GetByIdAsync(hogarId);
            return product is null ? NotFound(HttpMessages.NotFound) : Ok(product);
        }

    }
}
