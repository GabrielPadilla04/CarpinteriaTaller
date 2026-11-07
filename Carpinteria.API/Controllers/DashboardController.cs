using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP5_Servicios_API_REST.Services;
using TP5_Servicios_API_REST.Excepciones;

namespace TP5_Servicios_API_REST.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;

        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("perfil")]
        public async Task<IActionResult> ObtenerPerfil()
        {
            try
            {
                var perfil = await _dashboardService.ObtenerPerfilUsuarioAsync();
                return Ok(perfil);
            }
            catch (BaseDeDatosException ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        [HttpGet("visitas-hoy")]
        public async Task<IActionResult> ObtenerVisitasHoy()
        {
            try
            {
                var visitas = await _dashboardService.ObtenerVisitasHoyAsync();
                return Ok(visitas);
            }
            catch (BaseDeDatosException ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        [HttpGet("proyectos-activos")]
        public async Task<IActionResult> ObtenerProyectosActivos()
        {
            try
            {
                var proyectos = await _dashboardService.ObtenerProyectosActivosAsync();
                return Ok(proyectos);
            }
            catch (BaseDeDatosException ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }
    }
}