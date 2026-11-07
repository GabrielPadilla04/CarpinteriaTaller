using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TP5_Servicios_API_REST.Services;

namespace TP5_Servicios_API_REST.Controllers
{
    [Route("api/notificaciones")]
    [ApiController]
    [Authorize] // Protegido con token JWT
    public class NotificacionesController : ControllerBase
    {
        private readonly NotificacionService _notificacionService;

        public NotificacionesController(NotificacionService notificacionService)
        {
            _notificacionService = notificacionService;
        }

        // GET: api/notificaciones (Obtiene las notificaciones del usuario autenticado)
        [HttpGet]
        public async Task<IActionResult> ObtenerMisNotificaciones()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                  ?? User.FindFirst("id")?.Value;

                if (userIdClaim == null || !int.TryParse(userIdClaim, out int usuarioId))
                {
                    return Unauthorized(new { mensaje = "No se pudo identificar al usuario desde el token." });
                }

                var notificaciones = await _notificacionService.ObtenerPorUsuarioAsync(usuarioId);
                return Ok(notificaciones);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        // PATCH: api/notificaciones/{id}/leer (Marca una notificación como leída)
        [HttpPatch("{id}/leer")]
        public async Task<IActionResult> MarcarComoLeida(int id)
        {
            try
            {
                await _notificacionService.MarcarComoLeidaAsync(id);
                return Ok(new { mensaje = "Notificación marcada como leída exitosamente." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }
    }
}