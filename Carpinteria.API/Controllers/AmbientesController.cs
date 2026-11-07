using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP5_Servicios_API_REST.DTOs.Ambiente.Request;
using TP5_Servicios_API_REST.Services;
using TP5_Servicios_API_REST.Excepciones;

namespace TP5_Servicios_API_REST.Controllers
{
    [Route("api/ambientes")]
    [ApiController]
    [Authorize]
    public class AmbientesController : ControllerBase
    {
        private readonly AmbienteService _ambienteService;

        public AmbientesController(AmbienteService ambienteService)
        {
            _ambienteService = ambienteService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var ambientes = await _ambienteService.ObtenerTodos();
                return Ok(ambientes);
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
                var ambiente = await _ambienteService.ObtenerPorId(id);
                return Ok(ambiente);
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
        public async Task<IActionResult> Crear([FromBody] CrearAmbienteRequest dto)
        {
            try
            {
                var nuevoAmbiente = await _ambienteService.Crear(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoAmbiente.AmbienteId }, nuevoAmbiente);
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
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarAmbienteRequest dto)
        {
            try
            {
                await _ambienteService.Actualizar(id, dto);
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
                await _ambienteService.Eliminar(id);
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