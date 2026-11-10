using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TP5_Servicios_API_REST.DTOs.Material.Request;
using Carpinteria.API.DTOs.Material.Response;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Models;
using TP5_Servicios_API_REST.Services;

namespace TP5_Servicios_API_REST.Controllers
{
    [Route("api/materiales")]
    [ApiController]
    [Authorize]
    public class MaterialesController : ControllerBase // O MaterialesController
    {
        private readonly MaterialService _materialService;

        public MaterialesController(MaterialService materialService)
        {
            _materialService = materialService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MaterialListadoResponse>>> ObtenerTodos([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                return Ok(await _materialService.ObtenerTodos(page, pageSize));
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MaterialResponse>> ObtenerPorId(int id)
        {
            try
            {
                MaterialResponse dto = await _materialService.ObtenerPorId(id);
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
        public async Task<ActionResult<MaterialListadoResponse>> Crear(CrearMaterialRequest dto)
        {
            try
            {
                MaterialListadoResponse material = await _materialService.Crear(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = material.MaterialId }, material);
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(e.Message);
            }
            catch (RecursoExistenteException e)
            {
                return Conflict(e.Message);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, ActualizarMaterialRequest dto)
        {
            try
            {
                await _materialService.Actualizar(id, dto);
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
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                await _materialService.Eliminar(id);
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
    }
}