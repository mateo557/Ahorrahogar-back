using AhorrahogarApi.Constants;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Models;
using AhorrahogarApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AhorrahogarApi.Controllers;

// Requiere sesión. El hogar sale de quien inició sesión, no de lo que mande la app.
[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MovemntsController : ControllerBase
{
    private readonly IMovemntsService _service;
    private readonly IUsuarioActual _usuario;

    public MovemntsController(IMovemntsService service, IUsuarioActual usuario)
    {
        _service = service;
        _usuario = usuario;
    }

    private ObjectResult SinHogar() => StatusCode(403, "Primero únete a un hogar.");

    [HttpGet]
    public async Task<ActionResult<IEnumerable<MovemntsDto>>> Get([FromQuery] int? anio, [FromQuery] int? mes)
    {
        if (anio.HasValue != mes.HasValue)
            return BadRequest("Envía anio y mes juntos");

        if (mes < 1 || mes > 12 || anio < 1)
            return BadRequest("anio o mes fuera de rango");

        var hogarId = await _usuario.HogarIdAsync();
        if (hogarId is null) return SinHogar();

        return Ok(await _service.GetAllAsync(hogarId.Value, anio, mes));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovemntsDto>> GetById(int id)
    {
        var hogarId = await _usuario.HogarIdAsync();
        if (hogarId is null) return SinHogar();

        var mov = await _service.GetByIdAsync(id, hogarId.Value);
        return mov is null ? NotFound(HttpMessages.NotFound) : Ok(mov);
    }

    [HttpPost]
    public async Task<ActionResult<Response<object>>> Create(CreateMovemntsDto dto)
    {
        var hogarId = await _usuario.HogarIdAsync();
        if (hogarId is null) return SinHogar();

        try
        {
            await _service.CreateAsync(hogarId.Value, dto);
        }
        catch (KeyNotFoundException e)
        {
            return BadRequest(e.Message);
        }

        return StatusCode(201, new { message = HttpMessages.Confirm });
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Response<object>>> Update(int id, UpdateMovemntsDto dto)
    {
        var hogarId = await _usuario.HogarIdAsync();
        if (hogarId is null) return SinHogar();

        try
        {
            var updated = await _service.UpdateAsync(id, hogarId.Value, dto);
            return updated ? Ok(HttpMessages.Confirm) : NotFound(HttpMessages.NotFound);
        }
        catch (KeyNotFoundException e)
        {
            return BadRequest(e.Message);
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Response<object>>> Delete(int id)
    {
        var hogarId = await _usuario.HogarIdAsync();
        if (hogarId is null) return SinHogar();

        var delete = await _service.DeleteAsync(id, hogarId.Value);
        return delete ? Ok(HttpMessages.Confirm) : NotFound(HttpMessages.NotFound);
    }
}
