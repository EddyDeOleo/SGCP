using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGCP.Application.Dtos.ModuloUsuarios.Cliente;
using SGCP.Application.Interfaces.ModuloUsuarios;


namespace SGCP.ModuloUsuarios.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _service;  
        public ClienteController(IClienteService service)
        {
            _service = service;
        }
        [HttpGet("get-cliente")]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetCliente();

            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);

        }

        // GET api/<ReporteController>/5
        [HttpGet("getbyid-cliente")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _service.GetClienteById(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // POST api/<ReporteController>
        [HttpPost("create-cliente")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateClienteDTO createClienteDTO)
        {
            var result = await _service.CreateCliente(createClienteDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // PUT api/<ReporteController>/5
        [HttpPut("update-cliente")]
        [Authorize]
        public async Task<IActionResult> Put([FromBody] UpdateClienteDTO updateClienteDTO)
        {
            var result = await _service.UpdateCliente(updateClienteDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // DELETE api/<ReporteController>/5
        [HttpDelete("remove-cliente")]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] DeleteClienteDTO deleteClienteDTO)
        {
            var result = await _service.RemoveCliente(deleteClienteDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
