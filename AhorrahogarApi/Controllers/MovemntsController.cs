using AhorrahogarApi.Constants;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Services;
using AhorrahogarApi.Models;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;
using Google.Protobuf.WellKnownTypes;

namespace AhorrahogarApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MovemntsController : ControllerBase
{
    private readonly IMovemntsService _service;

    public MovemntsController(IMovemntsService service) => _service = service;

   
    [HttpGet]
    public async Task<ActionResult<Response<IEnumerable<MovemntsDto>>>> GetAll()
    {
        var movimientos = await _service.GetAllAsync();

        return Ok();
    }



    [HttpGet("{id:int}")]
    public async Task<ActionResult<MovemntsDto>> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);
        return product is null ? NotFound(HttpMessages.NotFound) : Ok();
    }

    [HttpPost]
    public async Task<ActionResult<Response<object>>> Create(CreateMovemntsDto dto)
    {
        await _service.CreateAsync(dto);

        return StatusCode(201,HttpMessages.Confirm);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<Response<object>>> Update(int id, UpdateMovemntsDto dto)
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
