using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TP5_Servicios_API_REST.DTOs.Ambiente.Request;
using TP5_Servicios_API_REST.DTOs.Ambiente.Response;

namespace TP5_Servicios_API_REST.Services
{
    public class AmbienteService
    {
        private readonly AppDbContext _context;
        private readonly BitacoraService _bitacoraService;
        private readonly NotificacionService _notificacionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AmbienteService(
            AppDbContext context,
            BitacoraService bitacoraService,
            NotificacionService notificacionService,
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

        public async Task<List<AmbienteResponse>> ObtenerTodos()
        {
            try
            {
                return await _context.Ambientes
                    .Select(a => new AmbienteResponse(
                        a.AmbienteId,
                        a.ProyectoId,
                        a.NombreAmbiente))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener los ambientes: {ex.Message}");
            }
        }

        public async Task<AmbienteResponse> ObtenerPorId(int id)
        {
            try
            {
                var ambiente = await _context.Ambientes.FindAsync(id);

                if (ambiente == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el ambiente con ID {id}.");
                }

                return new AmbienteResponse(
                    ambiente.AmbienteId,
                    ambiente.ProyectoId,
                    ambiente.NombreAmbiente);
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener el ambiente: {ex.Message}");
            }
        }

        public async Task<AmbienteResponse> Crear(CrearAmbienteRequest dto)
        {
            try
            {
                // Validar que el proyecto exista
                bool proyectoExiste = await _context.Proyectos.AnyAsync(p => p.ProyectoId == dto.ProyectoId);
                if (!proyectoExiste)
                {
                    throw new RecursoNoExisteException($"El proyecto con ID {dto.ProyectoId} no existe.");
                }

                Ambiente nuevoAmbiente = new Ambiente
                {
                    ProyectoId = dto.ProyectoId,
                    NombreAmbiente = dto.NombreAmbiente
                };

                _context.Ambientes.Add(nuevoAmbiente);
                await _context.SaveChangesAsync();

                // Obtener ID dinámico y registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se creó el ambiente '{dto.NombreAmbiente}' para el proyecto ID {dto.ProyectoId}",
                    modulo: "Ambientes"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Ambiente Creado",
                    mensaje: $"Creaste el ambiente '{dto.NombreAmbiente}' en el proyecto {dto.ProyectoId}."
                );

                return new AmbienteResponse(
                    nuevoAmbiente.AmbienteId,
                    nuevoAmbiente.ProyectoId,
                    nuevoAmbiente.NombreAmbiente);
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al crear el ambiente: {ex.Message}");
            }
        }

        public async Task Actualizar(int id, ActualizarAmbienteRequest dto)
        {
            try
            {
                var ambiente = await _context.Ambientes.FindAsync(id);
                if (ambiente == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el ambiente con ID {id}.");
                }

                bool proyectoExiste = await _context.Proyectos.AnyAsync(p => p.ProyectoId == dto.ProyectoId);
                if (!proyectoExiste)
                {
                    throw new RecursoNoExisteException($"El proyecto con ID {dto.ProyectoId} no existe.");
                }

                ambiente.ProyectoId = dto.ProyectoId;
                ambiente.NombreAmbiente = dto.NombreAmbiente;

                _context.Ambientes.Update(ambiente);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se actualizó el ambiente ID {id} con nombre '{dto.NombreAmbiente}'",
                    modulo: "Ambientes"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Ambiente Actualizado",
                    mensaje: $"Actualizaste el ambiente ID {id} a '{dto.NombreAmbiente}'."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al actualizar el ambiente: {ex.Message}");
            }
        }

        public async Task Eliminar(int id)
        {
            try
            {
                var ambiente = await _context.Ambientes.FindAsync(id);
                if (ambiente == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el ambiente con ID {id}.");
                }

                string nombreAmbiente = ambiente.NombreAmbiente;
                int proyectoId = ambiente.ProyectoId;

                _context.Ambientes.Remove(ambiente);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se eliminó el ambiente '{nombreAmbiente}' (ID {id}) del proyecto {proyectoId}",
                    modulo: "Ambientes"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Ambiente Eliminado",
                    mensaje: $"Eliminaste el ambiente '{nombreAmbiente}' del proyecto {proyectoId}."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al eliminar el ambiente: {ex.Message}");
            }
        }
    }
}