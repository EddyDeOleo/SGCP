using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SGCP.Application.Base;
using SGCP.Application.Dtos.ModuloProducto.Producto;
using SGCP.Application.Interfaces.ModuloProducto;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SGCP.ModuloProducto.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoService _productoService;
        public ProductoController(IProductoService productoService)
        {
            _productoService = productoService;
        }

        // GET: api/<ProductoController>
        [HttpGet("get-productos")]
        [Authorize]
        public async Task<IActionResult>  Get()
        {
            ServiceResult result = await _productoService.GetProducto();    

            if (!result.Success)
            {
                return BadRequest(result);
            }  
            return Ok(result);
        }


        // GET api/<ProductoController>/5
        [HttpGet("getbyid-productos")]
        [Authorize]
        public async Task<IActionResult> Get(int id)
        {
            ServiceResult result = await _productoService.GetProductoById(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // POST api/<ProductoController>
        [HttpPost("create-productos")]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] CreateProductoDTO createProductoDTO)
        {
            ServiceResult result = await _productoService.CreateProducto(createProductoDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // PUT api/<ProductoController>/5
        [HttpPut("update-productos")]
        [Authorize]
        public async Task<IActionResult> Put([FromBody] UpdateProductoDTO updateProductoDTO)
        {
            ServiceResult result = await _productoService.UpdateProducto(updateProductoDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // DELETE api/<ProductoController>/5
        [HttpDelete("remove-producto")]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] DeleteProductoDTO deleteProductoDTO)
        {
            ServiceResult result = await _productoService.RemoveProducto(deleteProductoDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
