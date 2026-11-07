using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TP5_Servicios_API_REST.Models;
using TP5_Servicios_API_REST.Services;

namespace TP5_Servicios_API_REST.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProveedoresController : ControllerBase
    {
        private readonly ProveedorService _proveedorService;

        public ProveedoresController(ProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var proveedores = await _proveedorService.ObtenerTodos();
            return Ok(proveedores);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var proveedor = await _proveedorService.ObtenerPorId(id);
            if (proveedor == null)
            {
                return NotFound();
            }
            return Ok(proveedor);
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] Proveedor proveedor)
        {
            var nuevoProveedor = await _proveedorService.Crear(proveedor);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoProveedor.IdProveedor }, nuevoProveedor);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Proveedor proveedor)
        {
            if (id != proveedor.IdProveedor)
            {
                return BadRequest("El ID del proveedor no coincide.");
            }

            var actualizado = await _proveedorService.Actualizar(id, proveedor);
            if (!actualizado)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var resultado = await _proveedorService.EliminarLogico(id);
            if (!resultado)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}