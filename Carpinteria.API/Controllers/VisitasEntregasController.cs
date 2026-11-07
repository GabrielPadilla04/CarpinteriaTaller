using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TP5_Servicios_API_REST.DTOs.VisitaEntrega.Request;
using TP5_Servicios_API_REST.DTOs.VisitaEntrega.Response;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Services;

namespace TP5_Servicios_API_REST.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class VisitasEntregasController : ControllerBase
	{
		private readonly VisitaEntregaService _visitaService;

		public VisitasEntregasController(VisitaEntregaService visitaService)
		{
			_visitaService = visitaService;
		}

		[HttpGet]
		public async Task<ActionResult<List<VisitaEntregaListadoResponse>>> ObtenerTodos([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			try
			{
				return Ok(await _visitaService.ObtenerTodos(page, pageSize));
			}
			catch (BaseDeDatosException e)
			{
				return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
			}
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<VisitaEntregaResponse>> ObtenerPorId(int id)
		{
			try
			{
				VisitaEntregaResponse dto = await _visitaService.ObtenerPorId(id);
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
		public async Task<ActionResult<VisitaEntregaListadoResponse>> Crear(CrearVisitaEntregaRequest dto)
		{
			try
			{
				VisitaEntregaListadoResponse visita = await _visitaService.Crear(dto);
				return CreatedAtAction(nameof(ObtenerPorId), new { id = visita.VisitaId }, visita);
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
		public async Task<IActionResult> Actualizar(int id, ActualizarVisitaEntregaRequest dto)
		{
			try
			{
				await _visitaService.Actualizar(id, dto);
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

		[HttpDelete("{id}")]
		public async Task<IActionResult> Eliminar(int id)
		{
			try
			{
				await _visitaService.Eliminar(id);
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