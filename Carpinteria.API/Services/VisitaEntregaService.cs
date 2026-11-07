using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.DTOs.VisitaEntrega.Request;
using TP5_Servicios_API_REST.DTOs.VisitaEntrega.Response;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Models;

namespace TP5_Servicios_API_REST.Services
{
    public class VisitaEntregaService
    {
        private readonly AppDbContext _context;
        private readonly BitacoraService _bitacoraService;
        private readonly NotificacionService _notificacionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public VisitaEntregaService(
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

        public async Task<List<VisitaEntregaListadoResponse>> ObtenerTodos(int page = 1, int pageSize = 10)
        {
            try
            {
                return await _context.VisitasEntregas
                    .Include(v => v.Proyecto)
                    .Include(v => v.Usuario)
                    .Skip((page - 1) * pageSize) // Corregido el parámetro de paginación
                    .Take(pageSize)
                    .Select(v => new VisitaEntregaListadoResponse(
                        v.VisitaId,
                        v.Proyecto != null ? v.Proyecto.NombreProyecto : "Sin proyecto",
                        v.Usuario != null ? $"{v.Usuario.Nombre} {v.Usuario.Apellido}" : "Sin asignar",
                        v.TipoEvento,
                        v.Fecha,
                        v.HoraInicio,
                        v.HoraFin))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener las visitas y entregas de la agenda: {ex.Message}");
            }
        }

        public async Task<VisitaEntregaResponse> ObtenerPorId(int id)
        {
            try
            {
                var visita = await _context.VisitasEntregas
                    .Include(v => v.Proyecto)
                    .Include(v => v.Usuario)
                    .FirstOrDefaultAsync(v => v.VisitaId == id);

                if (visita == null)
                {
                    throw new RecursoNoExisteException($"No se encontró la visita/entrega con ID {id}.");
                }

                return new VisitaEntregaResponse(
                    visita.VisitaId,
                    visita.ProyectoId,
                    visita.Proyecto?.NombreProyecto,
                    visita.UsuarioId,
                    $"{visita.Usuario?.Nombre} {visita.Usuario?.Apellido}",
                    visita.TipoEvento,
                    visita.Fecha,
                    visita.HoraInicio,
                    visita.HoraFin);
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener el detalle de la visita: {ex.Message}");
            }
        }

        public async Task<VisitaEntregaListadoResponse> Crear(CrearVisitaEntregaRequest dto)
        {
            try
            {
                bool proyectoExiste = await _context.Proyectos.AnyAsync(p => p.ProyectoId == dto.ProyectoId);
                if (!proyectoExiste)
                {
                    throw new RecursoNoExisteException($"El proyecto con ID {dto.ProyectoId} no existe.");
                }

                bool usuarioExiste = await _context.Usuarios.AnyAsync(u => u.UsuarioId == dto.UsuarioId);
                if (!usuarioExiste)
                {
                    throw new RecursoNoExisteException($"El operario encargado con ID {dto.UsuarioId} no existe.");
                }

                VisitaEntrega nuevaVisita = new VisitaEntrega
                {
                    ProyectoId = dto.ProyectoId,
                    UsuarioId = dto.UsuarioId,
                    TipoEvento = dto.TipoEvento,
                    Fecha = dto.Fecha,
                    HoraInicio = dto.HoraInicio,
                    HoraFin = dto.HoraFin
                };

                _context.VisitasEntregas.Add(nuevaVisita);
                await _context.SaveChangesAsync();

                await _context.Entry(nuevaVisita).Reference(v => v.Proyecto!).LoadAsync();
                await _context.Entry(nuevaVisita).Reference(v => v.Usuario!).LoadAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se programó una/un '{dto.TipoEvento}' (ID {nuevaVisita.VisitaId}) para el proyecto ID {dto.ProyectoId}",
                    modulo: "VisitasEntregas"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Visita/Entrega Programada",
                    mensaje: $"Programaste una/un '{dto.TipoEvento}' para el proyecto ID {dto.ProyectoId}."
                );

                return new VisitaEntregaListadoResponse(
                    nuevaVisita.VisitaId,
                    nuevaVisita.Proyecto?.NombreProyecto ?? "",
                    nuevaVisita.Usuario != null ? $"{nuevaVisita.Usuario.Nombre} {nuevaVisita.Usuario.Apellido}" : "",
                    nuevaVisita.TipoEvento,
                    nuevaVisita.Fecha,
                    nuevaVisita.HoraInicio,
                    nuevaVisita.HoraFin);
            }
            catch (RecursoNoExisteException) { throw; }
            catch (DbUpdateException ex)
            {
                string mensajeInner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new BaseDeDatosException($"Error exacto de Base de Datos: {mensajeInner}");
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al programar la visita/entrega: {ex.Message}");
            }
        }

        public async Task Actualizar(int id, ActualizarVisitaEntregaRequest dto)
        {
            try
            {
                var visita = await _context.VisitasEntregas.FindAsync(id);

                if (visita == null)
                {
                    throw new RecursoNoExisteException($"No se encontró la visita/entrega con ID {id}.");
                }

                if (visita.ProyectoId != dto.ProyectoId)
                {
                    bool proyectoExiste = await _context.Proyectos.AnyAsync(p => p.ProyectoId == dto.ProyectoId);
                    if (!proyectoExiste) throw new RecursoNoExisteException($"El proyecto con ID {dto.ProyectoId} no existe.");
                }

                if (visita.UsuarioId != dto.UsuarioId)
                {
                    bool usuarioExiste = await _context.Usuarios.AnyAsync(u => u.UsuarioId == dto.UsuarioId);
                    if (!usuarioExiste) throw new RecursoNoExisteException($"El operario con ID {dto.UsuarioId} no existe.");
                }

                visita.ProyectoId = dto.ProyectoId;
                visita.UsuarioId = dto.UsuarioId;
                visita.TipoEvento = dto.TipoEvento;
                visita.Fecha = dto.Fecha;
                visita.HoraInicio = dto.HoraInicio;
                visita.HoraFin = dto.HoraFin;

                _context.VisitasEntregas.Update(visita);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se actualizó la visita/entrega ID {id} ('{dto.TipoEvento}')",
                    modulo: "VisitasEntregas"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Visita/Entrega Actualizada",
                    mensaje: $"Actualizaste el evento '{dto.TipoEvento}' de la agenda."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (DbUpdateException ex)
            {
                string mensajeInner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new BaseDeDatosException($"Error exacto de Base de Datos: {mensajeInner}");
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al actualizar la visita/entrega: {ex.Message}");
            }
        }

        public async Task Eliminar(int id)
        {
            try
            {
                var visita = await _context.VisitasEntregas.FindAsync(id);

                if (visita == null)
                {
                    throw new RecursoNoExisteException($"No se encontró la visita/entrega con ID {id}.");
                }

                string tipoEvento = visita.TipoEvento;

                _context.VisitasEntregas.Remove(visita);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se eliminó de la agenda la/el '{tipoEvento}' (ID {id})",
                    modulo: "VisitasEntregas"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Visita/Entrega Eliminada",
                    mensaje: $"Eliminaste la/el '{tipoEvento}' de la agenda."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al eliminar la visita/entrega de la agenda: {ex.Message}");
            }
        }
    }
}