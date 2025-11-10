using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGCP.Application.Dtos.ModuloPedido.Pedido;
using SGCP.Application.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SGCP.ModuloPedido.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidoController : ControllerBase
    {

        private readonly IPedidoService _pedidoService;
        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }
        // GET: api/<PedidoController>
        [HttpGet("get-pedido")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var result = await _pedidoService.GetPedido();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // GET api/<PedidoController>/5
        [HttpGet("getbyid-pedido")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _pedidoService.GetPedidoById(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        // POST api/<PedidoController>
        [HttpPost("create-pedido")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreatePedidoDTO createPedidoDTO)
        {
            var result = await _pedidoService.CreatePedido(createPedidoDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        // PUT api/<PedidoController>/5
        [HttpPut("update-pedido")]
        [Authorize]
        public async Task<IActionResult> Put([FromBody] UpdatePedidoDTO updatePedidoDTO)
        {
            var result = await _pedidoService.UpdatePedido(updatePedidoDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // DELETE api/<PedidoController>/5
        [HttpDelete("remove-pedido")]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] DeletePedidoDTO deletePedidoDTO)
        {
            var result = await _pedidoService.RemovePedido(deletePedidoDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("finalizar-pedido")]
        [Authorize]
        public async Task<IActionResult> FinalizarPedido(int idPedido)
        {
            var result = await _pedidoService.FinalizarPedido(idPedido);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("cancelar-pedido")]
        [Authorize]
        public async Task<IActionResult> CancelarPedido(int idPedido)
        {
            var result = await _pedidoService.CancelarPedido(idPedido);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }


    }
}
