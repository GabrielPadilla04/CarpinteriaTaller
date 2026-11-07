using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.DTOs.ProyectoMaterial.Request;
using TP5_Servicios_API_REST.DTOs.ProyectoMaterial.Response;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Models;

namespace TP5_Servicios_API_REST.Services
{
    public class ProyectoMaterialService
    {
        private readonly AppDbContext _context;
        private readonly BitacoraService _bitacoraService;
        private readonly NotificacionService _notificacionService; // <-- 1. Inyectamos notificaciones
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProyectoMaterialService(
            AppDbContext context,
            BitacoraService bitacoraService,
            NotificacionService notificacionService, // <-- 2. Lo recibimos en el constructor
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _bitacoraService = bitacoraService;
            _notificacionService = notificacionService;
            _httpContextAccessor = httpContextAccessor;
        }

        // Método auxiliar para extraer automáticamente el ID del usuario del Token JWT actual
        private int ObtenerUsuarioIdActual()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                              ?? _httpContextAccessor.HttpContext?.User?.FindFirst("id");

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int usuarioId))
            {
                return usuarioId;
            }

            throw new UnauthorizedAccessException("No se pudo obtener el ID del usuario desde el token JWT.");
        }

        public async Task<List<ProyectoMaterialResponse>> ObtenerPorProyecto(int proyectoId)
        {
            try
            {
                var existeProyecto = await _context.Proyectos.AnyAsync(p => p.ProyectoId == proyectoId);
                if (!existeProyecto)
                {
                    throw new RecursoNoExisteException($"No se encontró el proyecto con ID {proyectoId}.");
                }

                return await _context.ProyectosMateriales
                    .Where(pm => pm.ProyectoId == proyectoId)
                    .Include(pm => pm.Material)
                    .Select(pm => new ProyectoMaterialResponse(
                        pm.ProyectoId,
                        pm.MaterialId,
                        pm.Material.NombreColorDiseno,
                        pm.Material.MarcaProveedor,
                        pm.Material.Acabado ?? "Sin especificar",
                        pm.Cantidad))
                    .ToListAsync();
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener los materiales del proyecto: {ex.Message}");
            }
        }

        public async Task<ProyectoMaterialResponse> AsignarMaterial(AsignarMaterialProyectoRequest dto)
        {
            try
            {
                var existeProyecto = await _context.Proyectos.AnyAsync(p => p.ProyectoId == dto.ProyectoId);
                if (!existeProyecto)
                {
                    throw new RecursoNoExisteException($"No se encontró el proyecto con ID {dto.ProyectoId}.");
                }

                var material = await _context.Materiales.FindAsync(dto.MaterialId);
                if (material == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el material con ID {dto.MaterialId}.");
                }

                var asignacionExistente = await _context.ProyectosMateriales
                    .AsNoTracking()
                    .FirstOrDefaultAsync(pm => pm.ProyectoId == dto.ProyectoId && pm.MaterialId == dto.MaterialId);

                if (asignacionExistente != null)
                {
                    throw new RecursoExistenteException("El material ya está asignado a este proyecto. Use PUT para actualizar la cantidad.");
                }

                var nuevoPm = new ProyectoMaterial
                {
                    ProyectoId = dto.ProyectoId,
                    MaterialId = dto.MaterialId,
                    Cantidad = dto.Cantidad
                };

                _context.ProyectosMateriales.Add(nuevoPm);
                await _context.SaveChangesAsync();

                // Obtenemos el ID dinámico del usuario autenticado vía JWT
                int usuarioIdActual = ObtenerUsuarioIdActual();

                // 1. REGISTRAR EN BITÁCORA
                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se asignó el material ID {dto.MaterialId} al proyecto {dto.ProyectoId} con cantidad {dto.Cantidad}",
                    modulo: "ProyectosMateriales"
                );

                // 2. ENVIAR NOTIFICACIÓN AUTOMÁTICA
                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Material Asignado",
                    mensaje: $"Has asignado el material ID {dto.MaterialId} al proyecto {dto.ProyectoId}."
                );

                return new ProyectoMaterialResponse(
                    nuevoPm.ProyectoId,
                    nuevoPm.MaterialId,
                    material.NombreColorDiseno,
                    material.MarcaProveedor,
                    material.Acabado ?? "Sin especificar",
                    nuevoPm.Cantidad);
            }
            catch (RecursoNoExisteException) { throw; }
            catch (RecursoExistenteException) { throw; }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR EN ASIGNAR MATERIAL]: {ex.InnerException?.Message ?? ex.Message}");
                throw new BaseDeDatosException($"Error al asignar el material al proyecto: {ex.Message}");
            }
        }

        public async Task ActualizarCantidad(int proyectoId, int materialId, ActualizarCantidadMaterialRequest dto)
        {
            try
            {
                var asignacion = await _context.ProyectosMateriales
                    .FirstOrDefaultAsync(pm => pm.ProyectoId == proyectoId && pm.MaterialId == materialId);

                if (asignacion == null)
                {
                    throw new RecursoNoExisteException("No se encontró la asignación de este material en el proyecto.");
                }

                asignacion.Cantidad = dto.Cantidad;
                _context.ProyectosMateriales.Update(asignacion);
                await _context.SaveChangesAsync();

                int usuarioIdActual = ObtenerUsuarioIdActual();

                // 1. REGISTRAR EN BITÁCORA
                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se actualizó la cantidad del material ID {materialId} en el proyecto {proyectoId} a {dto.Cantidad}",
                    modulo: "ProyectosMateriales"
                );

                // 2. ENVIAR NOTIFICACIÓN AUTOMÁTICA
                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Cantidad Actualizada",
                    mensaje: $"Actualizaste la cantidad del material ID {materialId} en el proyecto {proyectoId} a {dto.Cantidad}."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al actualizar la cantidad del material: {ex.Message}");
            }
        }

        public async Task RemoverMaterial(int proyectoId, int materialId)
        {
            try
            {
                var asignacion = await _context.ProyectosMateriales
                    .FirstOrDefaultAsync(pm => pm.ProyectoId == proyectoId && pm.MaterialId == materialId);

                if (asignacion == null)
                {
                    throw new RecursoNoExisteException("No se encontró la asignación de este material en el proyecto.");
                }

                _context.ProyectosMateriales.Remove(asignacion);
                await _context.SaveChangesAsync();

                int usuarioIdActual = ObtenerUsuarioIdActual();

                // 1. REGISTRAR EN BITÁCORA
                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se removió el material ID {materialId} del proyecto {proyectoId}",
                    modulo: "ProyectosMateriales"
                );

                // 2. ENVIAR NOTIFICACIÓN AUTOMÁTICA
                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Material Removido",
                    mensaje: $"Removiste el material ID {materialId} del proyecto {proyectoId}."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al eliminar el material del proyecto: {ex.Message}");
            }
        }
    }
}