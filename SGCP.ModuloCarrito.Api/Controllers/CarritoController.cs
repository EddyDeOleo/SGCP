using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGCP.Application.Dtos.ModuloCarrito.Carrito;
using SGCP.Application.Dtos.ModuloCarrito.CarritoProducto;
using SGCP.Application.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SGCP.ModuloCarrito.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarritoController : ControllerBase
    {

        private readonly ICarritoService _carritoService;
        public CarritoController(ICarritoService carritoService)
        {
            _carritoService = carritoService;
        }

        // GET: api/<CarritoController>
        [HttpGet("get-carrito")]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var result = await _carritoService.GetCarrito();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // GET api/<CarritoController>/5
        [HttpGet("getbyid-carrito")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _carritoService.GetCarritoById(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // POST api/<CarritoController>
        [HttpPost("create-carrito")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateCarritoDTO createCarritoDTO)
        {
            var result = await _carritoService.CreateCarrito(createCarritoDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // PUT api/<CarritoController>/5
        [HttpPut("update-carrito")]
        [Authorize]
        public async Task<IActionResult> Put([FromBody] UpdateCarritoDTO updateCarritoDTO)
        {
            var result = await _carritoService.UpdateCarrito(updateCarritoDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // DELETE api/<CarritoController>/5
        [HttpDelete("remove-carrito")]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] DeleteCarritoDTO deleteCarritoDTO)
        {
            var result = await _carritoService.RemoveCarrito(deleteCarritoDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
        [HttpPost("agregar-productos-al-carrito")]
        [Authorize]
        public async Task<IActionResult> AgregarProducto(int carritoId, [FromBody] AgregarProductoDTO agregarProductoDTO)
        {

            var result = await _carritoService.AgregarProductoAlCarrito(carritoId, agregarProductoDTO);

            return result.Success ? Ok(result) : BadRequest(result);
        }


    }
}
