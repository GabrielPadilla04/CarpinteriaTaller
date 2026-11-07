using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TP5_Servicios_API_REST.DTOs.EstadoProyecto.Request;
using TP5_Servicios_API_REST.DTOs.EstadoProyecto.Response;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Services;

namespace TP5_Servicios_API_REST.Controllers
{
    [Route("api/estados-proyecto")]
    [ApiController]
    [Authorize]
    public class EstadosProyectoController : ControllerBase
    {
        private readonly EstadoProyectoService _estadoService;

        public EstadosProyectoController(EstadoProyectoService estadoService)
        {
            _estadoService = estadoService;
        }

        [HttpGet]
        public async Task<ActionResult<List<EstadoProyectoResponse>>> ObtenerTodos()
        {
            try
            {
                var lista = await _estadoService.ObtenerTodos();
                return Ok(lista);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EstadoProyectoResponse>> ObtenerPorId(int id)
        {
            try
            {
                var estado = await _estadoService.ObtenerPorId(id);
                return Ok(estado);
            }
            catch (RecursoNoExisteException e)
            {
                return NotFound(new { mensaje = e.Message });
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<EstadoProyectoResponse>> Crear([FromBody] CrearEstadoProyectoRequest dto)
        {
            try
            {
                var estado = await _estadoService.Crear(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = estado.EstadoId }, estado);
            }
            catch (RecursoExistenteException e)
            {
                return Conflict(new { mensaje = e.Message });
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarEstadoProyectoRequest dto)
        {
            try
            {
                await _estadoService.Actualizar(id, dto);
                return NoContent();
            }
            catch (RecursoNoExisteException e)
            {
                return NotFound(new { mensaje = e.Message });
            }
            catch (RecursoExistenteException e)
            {
                return Conflict(new { mensaje = e.Message });
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _estadoService.Eliminar(id);
                return NoContent();
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(new { mensaje = e.Message });
            }
            catch (RecursoNoExisteException e)
            {
                return NotFound(new { mensaje = e.Message });
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message });
            }
        }
    }
}