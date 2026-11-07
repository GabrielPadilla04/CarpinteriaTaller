using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP5_Servicios_API_REST.DTOs.Medicion.Request;
using TP5_Servicios_API_REST.Services;
using TP5_Servicios_API_REST.Excepciones;

namespace TP5_Servicios_API_REST.Controllers
{
    [Route("api/mediciones")]
    [ApiController]
    [Authorize]
    public class MedicionesController : ControllerBase
    {
        private readonly MedicionService _medicionService;

        public MedicionesController(MedicionService medicionService)
        {
            _medicionService = medicionService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                var mediciones = await _medicionService.ObtenerTodas(); // Asegúrate de tener este método en tu servicio si lo deseas
                return Ok(mediciones);
            }
            catch (BaseDeDatosException ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        [HttpGet("ambiente/{ambienteId}")]
        public async Task<IActionResult> ObtenerPorAmbiente(int ambienteId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var mediciones = await _medicionService.ObtenerPorAmbiente(ambienteId, page, pageSize);
                return Ok(mediciones);
            }
            catch (BaseDeDatosException ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            try
            {
                var medicion = await _medicionService.ObtenerPorId(id);
                return Ok(medicion);
            }
            catch (RecursoNoExisteException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (BaseDeDatosException ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] CrearMedicionRequest dto)
        {
            try
            {
                var nuevaMedicion = await _medicionService.Crear(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaMedicion.MedicionId }, nuevaMedicion);
            }
            catch (RecursoNoExisteException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (BaseDeDatosException ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarMedicionRequest dto)
        {
            try
            {
                await _medicionService.Actualizar(id, dto);
                return NoContent();
            }
            catch (RecursoNoExisteException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (BaseDeDatosException ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _medicionService.Eliminar(id);
                return NoContent();
            }
            catch (RecursoNoExisteException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (BaseDeDatosException ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }
    }
}