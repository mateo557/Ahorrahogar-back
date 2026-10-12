using Microsoft.AspNetCore.Mvc;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Services;

namespace AhorrahogarApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovemntsController : ControllerBase
{
    private readonly IMovemntsService _service;

    public MovemntsController(IMovemntsService service) => _service = service;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovemntsDto>>> GetAll() =>
        Ok(await _service.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovemntsDto>> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<MovemntsDto>> Create(CreateMovemntsDto dto)
    {
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, UpdateMovemntsDto dto) =>
        await _service.UpdateAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await _service.DeleteAsync(id) ? NoContent() : NotFound();
}
