using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP5_Servicios_API_REST.Services;

namespace TP5_Servicios_API_REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EstadosTallerController : ControllerBase
    {
        private readonly EstadoTallerService _estadoTallerService;

        public EstadosTallerController(EstadoTallerService estadoTallerService)
        {
            _estadoTallerService = estadoTallerService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var estados = await _estadoTallerService.ObtenerTodos();
            return Ok(estados);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var estado = await _estadoTallerService.ObtenerPorId(id);
            return Ok(estado);
        }
    }
}