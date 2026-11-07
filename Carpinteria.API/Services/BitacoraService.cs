using Microsoft.EntityFrameworkCore;
using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.DTOs.Bitacora.Response;
using TP5_Servicios_API_REST.Models;
using TP5_Servicios_API_REST.Excepciones;

namespace TP5_Servicios_API_REST.Services
{
    public class BitacoraService
    {
        private readonly AppDbContext _context;

        public BitacoraService(AppDbContext context)
        {
            _context = context;
        }

        // Método auxiliar para registrar acciones con depuración ampliada
        public async Task RegistrarAccionAsync(int usuarioId, string accion, string modulo)
        {
            try
            {
                Console.WriteLine($"[BITÁCORA] Intentando registrar acción para el usuario {usuarioId} en el módulo {modulo}...");

                var bitacora = new BitacoraActividad
                {
                    UsuarioId = usuarioId,
                    DescripcionAccion = accion,
                    ModuloAfectado = modulo,
                    FechaHora = DateTime.Now
                };

                _context.BitacoraActividades.Add(bitacora);
                await _context.SaveChangesAsync();

                Console.WriteLine("[BITÁCORA] ¡Acción registrada y guardada con éxito en la base de datos!");
            }
            catch (Exception ex)
            {
                // Capturamos el error real y la excepción interna si la base de datos rechaza la inserción
                var errorDetalle = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine($"[ERROR AL REGISTRAR BITACORA]: {errorDetalle}");
            }
        }

        // Método para listar toda la auditoría (ordenada de la más reciente a la más antigua)
        public async Task<List<BitacoraResponse>> ObtenerTodasAsync()
        {
            try
            {
                return await _context.BitacoraActividades
                    .OrderByDescending(b => b.FechaHora)
                    .Select(b => new BitacoraResponse(
                        b.LogId,
                        b.UsuarioId,
                        b.DescripcionAccion,
                        b.ModuloAfectado,
                        b.FechaHora))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener la bitácora: {ex.Message}");
            }
        }
    }
}