using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TP5_Servicios_API_REST.DTOs.Proyecto.Request;
using TP5_Servicios_API_REST.DTOs.Proyecto.Response;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Models;
using TP5_Servicios_API_REST.Services;

namespace TP5_Servicios_API_REST.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class ProyectosController : ControllerBase
	{
		private readonly ProyectoService _proyectoService;

		public ProyectosController(ProyectoService proyectoService)
		{
			_proyectoService = proyectoService;
		}

		[HttpGet]
		public async Task<ActionResult<List<ProyectoListadoResponse>>> ObtenerTodos([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			try
			{
				return Ok(await _proyectoService.ObtenerTodos(page, pageSize));
			}
			catch (BaseDeDatosException e)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<ProyectoResponse>> ObtenerPorId(int id)
		{
			try
			{
				ProyectoResponse dto = await _proyectoService.ObtenerPorId(id);
				return Ok(dto);
			}
			catch (RecursoNoExisteException e)
			{
				return NotFound(e.Message);
			}
			catch (BaseDeDatosException e)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}
		}

		[HttpPost]
		public async Task<ActionResult<ProyectoListadoResponse>> Crear(CrearProyectoRequest dto)
		{
			try
			{
				ProyectoListadoResponse proyecto = await _proyectoService.Crear(dto);
				return CreatedAtAction(nameof(ObtenerPorId), new { id = proyecto.ProyectoId }, proyecto);
			}
			catch (DatosLlegaronErradosException e)
			{
				return BadRequest(e.Message);
			}
			catch (RecursoNoExisteException e)
			{
				return NotFound(e.Message);
			}
			catch (BaseDeDatosException e)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Actualizar(int id, ActualizarProyectoRequest dto)
		{
			try
			{
				await _proyectoService.Actualizar(id, dto);
				return NoContent();
			}
			catch (DatosLlegaronErradosException e)
			{
				return BadRequest(e.Message);
			}
			catch (RecursoNoExisteException e)
			{
				return NotFound(e.Message);
			}
			catch (BaseDeDatosException e)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}
		}

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Eliminar(int id)
		{
			try
			{
				await _proyectoService.Eliminar(id);
				return NoContent();
			}
			catch (RecursoNoExisteException e)
			{
				return NotFound(e.Message);
			}
			catch (BaseDeDatosException e)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}
		}
	}
}