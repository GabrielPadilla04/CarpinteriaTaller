using TP5_Servicios_API_REST.DTOs.Usuario.Input;
using TP5_Servicios_API_REST.DTOs.Usuario.Output;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Services;
using TP5_Servicios_API_REST.Models;
using TP5_Servicios_API_REST.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace TP5_Servicios_API_REST.Controllers
{
    [Route("api/usuarios")]
    [ApiController]
    [Authorize]
    public class UsuariosController : ControllerBase
    {
        private readonly UsuarioService _usuarioService;
        private readonly AppDbContext _context;

        public UsuariosController(UsuarioService usuarioService, AppDbContext context)
        {
            _usuarioService = usuarioService;
            _context = context;
        }

        // ----------------------------------------------------
        // ENDPOINT: Perfil del Usuario Autenticado (Solo Nombre)
        // ----------------------------------------------------
        [HttpGet("perfil")]
        public async Task<IActionResult> ObtenerPerfilPropio()
        {
            try
            {
                string? idClaimUsuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                     ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (!int.TryParse(idClaimUsuario, out int id))
                {
                    return Unauthorized("No se pudo identificar al usuario.");
                }

                var perfil = await _context.Usuarios
                    .Include(u => u.Roles)
                    .Where(u => u.UsuarioId == id)
                    .Select(u => new
                    {
                        Nombre = u.Nombre, // Únicamente el nombre de pila
                        Rol = u.Roles != null ? u.Roles.NombreRoles : "Carpintero / Diseñador"
                    })
                    .FirstOrDefaultAsync();

                if (perfil == null)
                {
                    return NotFound("Usuario no encontrado.");
                }

                return Ok(perfil);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        // ----------------------------------------------------
        // ENDPOINTS DE USUARIO (Acceso propio o General)
        // ----------------------------------------------------

        [HttpGet]
        public async Task<ActionResult<List<UsuarioDtoOutput>>> ObtenerTodos([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                List<UsuarioDtoOutput> usuarios = await _usuarioService.ObtenerlosATodos(page, pageSize);
                return Ok(usuarios);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        [HttpDelete("dardebaja")]
        public async Task<ActionResult> DarDeBajaPropia()
        {
            try
            {
                string? idClaimUsuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                     ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (!int.TryParse(idClaimUsuario, out int id))
                {
                    return Unauthorized("No se pudo identificar al usuario.");
                }

                await _usuarioService.DarDeBaja(id);
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

        [HttpPut("actualizar")]
        public async Task<IActionResult> ActualizarPropio([FromBody] UsuarioDtoInput usuarioDtoInput)
        {
            try
            {
                string? idClaimUsuario = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                     ?? User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (!int.TryParse(idClaimUsuario, out int id))
                {
                    return Unauthorized("No se pudo identificar al usuario.");
                }

                await _usuarioService.Actualizar(id, usuarioDtoInput);
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
            catch (RecursoExistenteException e)
            {
                return Conflict(e.Message);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }

        // ----------------------------------------------------
        // ENDPOINTS EXCLUSIVOS DE ADMINISTRADOR
        // ----------------------------------------------------

        [HttpPost("asignar-rol/{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> AsignarRol(int id, [FromBody] int nuevoRoleId)
        {
            try
            {
                await _usuarioService.AsignarRol(id, nuevoRoleId);
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

        [HttpDelete("admin/dardebaja/{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> DarDeBajaAdmin(int id)
        {
            try
            {
                await _usuarioService.DarDeBaja(id);
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

        [HttpPut("admin/actualizar/{id:int}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult> ActualizarAdmin(int id, [FromBody] UsuarioDtoInput usuarioDtoInput)
        {
            try
            {
                await _usuarioService.Actualizar(id, usuarioDtoInput);
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
            catch (RecursoExistenteException e)
            {
                return Conflict(e.Message);
            }
            catch (BaseDeDatosException e)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, e.Message);
            }
        }
    }
}