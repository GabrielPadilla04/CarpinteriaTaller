using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TP5_Servicios_API_REST.DTOs.ProyectoMaterial.Request;
using TP5_Servicios_API_REST.DTOs.ProyectoMaterial.Response;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Services;

namespace TP5_Servicios_API_REST.Controllers
{
	[Route("api/proyectos-materiales")]
	[ApiController]
	[Authorize]
	public class ProyectosMaterialesController : ControllerBase
	{
		private readonly ProyectoMaterialService _service;

		public ProyectosMaterialesController(ProyectoMaterialService service)
		{
			_service = service;
		}

		[HttpGet("proyecto/{proyectoId}")]
		public async Task<ActionResult<List<ProyectoMaterialResponse>>> ObtenerPorProyecto(int proyectoId)
		{
			try
			{
				var lista = await _service.ObtenerPorProyecto(proyectoId);
				return Ok(lista);
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
		public async Task<ActionResult<ProyectoMaterialResponse>> AsignarMaterial([FromBody] AsignarMaterialProyectoRequest dto)
		{
			try
			{
				var resultado = await _service.AsignarMaterial(dto);
				return StatusCode(StatusCodes.Status201Created, resultado);
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

		[HttpPut("proyecto/{proyectoId}/material/{materialId}")]
		public async Task<IActionResult> ActualizarCantidad(int proyectoId, int materialId, [FromBody] ActualizarCantidadMaterialRequest dto)
		{
			try
			{
				await _service.ActualizarCantidad(proyectoId, materialId, dto);
				return NoContent();
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

		[HttpDelete("proyecto/{proyectoId}/material/{materialId}")]
		public async Task<IActionResult> RemoverMaterial(int proyectoId, int materialId)
		{
			try
			{
				await _service.RemoverMaterial(proyectoId, materialId);
				return NoContent();
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