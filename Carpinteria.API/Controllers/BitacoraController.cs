using Microsoft.AspNetCore.Mvc;
using TP5_Servicios_API_REST.DTOs.Bitacora.Response;
using TP5_Servicios_API_REST.Services;

namespace TP5_Servicios_API_REST.Controllers
{
    [ApiController]
    [Route("api/bitacora")]
    public class BitacoraController : ControllerBase
    {
        private readonly BitacoraService _bitacoraService;

        public BitacoraController(BitacoraService bitacoraService)
        {
            _bitacoraService = bitacoraService;
        }

        [HttpGet]
        public async Task<ActionResult<List<BitacoraResponse>>> ObtenerBitacora()
        {
            var registros = await _bitacoraService.ObtenerTodasAsync();
            return Ok(registros);
        }
    }
}