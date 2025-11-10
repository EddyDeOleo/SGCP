using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGCP.Application.Dtos.ModuloUsuarios.Administrador;
using SGCP.Application.Interfaces.ModuloUsuarios;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SGCP.ModuloUsuarios.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdministradorController : ControllerBase
    {

        private readonly IAdminService _service;
        public AdministradorController(IAdminService service)
        {
            _service = service;
        }

        // GET: api/<AdministradorController>
        [HttpGet("get-admin")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetAdmin();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // GET api/<AdministradorController>/5
        [HttpGet("getbyid-admin")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.GetAdminById(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // POST api/<AdministradorController>
        [HttpPost("create-admin")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateAdminDTO createAdminDTO)
        {
            var result = await _service.CreateAdmin(createAdminDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }   

        // PUT api/<AdministradorController>/5
        [HttpPut("update-admin")]
        [Authorize]
        public async Task<IActionResult> Put([FromBody] UpdateAdminDTO updateAdminDTO)
        {
            var result = await _service.UpdateAdmin(updateAdminDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // DELETE api/<AdministradorController>/5
        [HttpDelete("remove-admin")]
        public async Task<IActionResult> Delete([FromBody] DeleteAdminDTO deleteAdminDTO)
        {
            var result = await _service.RemoveAdmin(deleteAdminDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    
    }
}
