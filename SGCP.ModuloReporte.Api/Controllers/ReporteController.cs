using Microsoft.AspNetCore.Mvc;
using SGCP.Application.Dtos.ModuloReporte.Reporte;
using SGCP.Application.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SGCP.ModuloReporte.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReporteController : ControllerBase
    {

        private readonly IReporteService _reporteService;
        public ReporteController(IReporteService reporteService)
        {
            _reporteService = reporteService;
        }

        // GET: api/<ReporteController>
        [HttpGet("get-reporte")]
        public async Task<IActionResult> Get()
        {
            var result = await _reporteService.GetReporte();
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // GET api/<ReporteController>/5
        [HttpGet("getbyid-reporte")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _reporteService.GetReporteById(id);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        // POST api/<ReporteController>
        [HttpPost("create-reporte")]
        public async Task<IActionResult> Post([FromBody] CreateReporteDTO createReporteDTO)
        {
            var result = await _reporteService.CreateReporte(createReporteDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }   

        // PUT api/<ReporteController>/5
        [HttpPut("update-reporte")]
        public async Task<IActionResult> Put([FromBody] UpdateReporteDTO updateReporteDTO)
        {
            var result = await _reporteService.UpdateReporte(updateReporteDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }   

        // DELETE api/<ReporteController>/5
        [HttpDelete("remove-reporte")]
        public async Task<IActionResult> Delete([FromBody] DeleteReporteDTO deleteReporteDTO)
        {
            var result = await _reporteService.RemoveReporte(deleteReporteDTO);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }
    }
}
