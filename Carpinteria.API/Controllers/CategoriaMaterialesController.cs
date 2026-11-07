using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TP5_Servicios_API_REST.DTOs.CategoriaMaterial.Request;
using TP5_Servicios_API_REST.DTOs.CategoriaMaterial.Response;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Services;

namespace TP5_Servicios_API_REST.Controllers
{
    [Route("api/categorias-material")]
    [ApiController]
    [Authorize]
    public class CategoriasMaterialController : ControllerBase
    {
        private readonly CategoriaMaterialService _categoriaService;

        public CategoriasMaterialController(CategoriaMaterialService categoriaService)
        {
            _categoriaService = categoriaService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CategoriaMaterialResponse>>> ObtenerTodos([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var lista = await _categoriaService.ObtenerTodos(page, pageSize);
                return Ok(lista);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = e.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaMaterialResponse>> ObtenerPorId(int id)
        {
            try
            {
                CategoriaMaterialResponse categoria = await _categoriaService.ObtenerPorId(id);
                return Ok(categoria);
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
        public async Task<ActionResult<CategoriaMaterialResponse>> Crear([FromBody] CrearCategoriaMaterialRequest dto)
        {
            try
            {
                CategoriaMaterialResponse categoria = await _categoriaService.Crear(dto);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = categoria.CategoriaId }, categoria);
            }
            catch (DatosLlegaronErradosException e)
            {
                return BadRequest(new { mensaje = e.Message });
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
        public async Task<IActionResult> Actualizar(int id, [FromBody] ActualizarCategoriaMaterialRequest dto)
        {
            try
            {
                await _categoriaService.Actualizar(id, dto);
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
                await _categoriaService.Eliminar(id);
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
            catch (Exception e)
            {
                return BadRequest(new { mensaje = $"No se puede eliminar la categoría: {e.Message}" });
            }
        }
    }
}