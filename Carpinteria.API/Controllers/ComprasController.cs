using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP5_Servicios_API_REST.Models;
using TP5_Servicios_API_REST.Services;

namespace TP5_Servicios_API_REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ComprasController : ControllerBase
    {
        private readonly CompraService _compraService;

        public ComprasController(CompraService compraService)
        {
            _compraService = compraService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodas()
        {
            var compras = await _compraService.ObtenerTodas();
            return Ok(compras);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var compra = await _compraService.ObtenerPorId(id);
            if (compra == null)
            {
                return NotFound();
            }
            return Ok(compra);
        }

        [HttpPost]
        public async Task<IActionResult> Registrar([FromBody] Compra compra)
        {
            var nuevaCompra = await _compraService.Registrar(compra);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaCompra.CompraId }, nuevaCompra);
        }
    }
}