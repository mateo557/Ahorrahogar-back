using AhorrahogarApi.Constants;
using AhorrahogarApi.DTOs;
using AhorrahogarApi.Models;
using AhorrahogarApi.Services;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Asn1.Ocsp;

namespace AhorrahogarApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly IMemberService _service;

        public MembersController(IMemberService service) => _service = service;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetAll() =>
            Ok(await _service.GetAllAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MemberDto>> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            return product is null ? NotFound(HttpMessages.NotFound) : Ok();
        }

        [HttpPost]
        public async Task<ActionResult<MemberDto>> Create(CreateMemberDto dto)
        {

            await _service.CreateAsync(dto);

            return StatusCode(201, HttpMessages.Confirm);

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<Response<object>>> Update(int id, UpdateMemberDto dto)
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
