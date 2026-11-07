using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.DTOs.Proyecto.Request;
using TP5_Servicios_API_REST.DTOs.Proyecto.Response;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Models;

namespace TP5_Servicios_API_REST.Services
{
    public class ProyectoService
    {
        private readonly AppDbContext _context;
        private readonly BitacoraService _bitacoraService;
        private readonly NotificacionService _notificacionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProyectoService(
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

        public async Task<List<ProyectoListadoResponse>> ObtenerTodos(int page = 1, int pageSize = 10)
        {
            try
            {
                return await _context.Proyectos
                    .Include(p => p.Cliente)
                    .Include(p => p.Usuario)
                    .Include(p => p.EstadoProyecto)
                    .Include(p => p.EstadoTaller) // Incluimos la nueva relación
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(p => new ProyectoListadoResponse(
                        p.ProyectoId,
                        p.NombreProyecto,
                        p.Cliente != null ? p.Cliente.NombreApellido : "Sin cliente",
                        p.Usuario != null ? $"{p.Usuario.Nombre} {p.Usuario.Apellido}" : "Sin asignar",
                        p.EstadoProyecto != null ? p.EstadoProyecto.NombreEstado : "Sin estado",
                        p.EstadoTaller != null ? p.EstadoTaller.NombreEstadoTaller : "Sin estado de taller",
                        p.FechaCreacion))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener los proyectos: {ex.Message}");
            }
        }

        public async Task<ProyectoResponse> ObtenerPorId(int id)
        {
            try
            {
                var proyecto = await _context.Proyectos
                    .Include(p => p.Cliente)
                    .Include(p => p.Usuario)
                    .Include(p => p.EstadoProyecto)
                    .Include(p => p.EstadoTaller) // Incluimos la nueva relación
                    .FirstOrDefaultAsync(p => p.ProyectoId == id);

                if (proyecto == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el proyecto con ID {id}.");
                }

                return new ProyectoResponse(
                    proyecto.ProyectoId,
                    proyecto.ClienteId,
                    proyecto.Cliente?.NombreApellido,
                    proyecto.UsuarioId,
                    $"{proyecto.Usuario?.Nombre} {proyecto.Usuario?.Apellido}",
                    proyecto.NombreProyecto,
                    proyecto.EstadoId,
                    proyecto.EstadoProyecto?.NombreEstado,
                    proyecto.EstadoTaller?.NombreEstadoTaller,
                    proyecto.Observaciones,
                    proyecto.FechaCreacion);
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener el proyecto: {ex.Message}");
            }
        }

        public async Task<ProyectoListadoResponse> Crear(CrearProyectoRequest dto)
        {
            try
            {
                bool clienteExiste = await _context.Clientes.AnyAsync(c => c.ClienteId == dto.ClienteId);
                if (!clienteExiste)
                {
                    throw new RecursoNoExisteException($"El cliente con ID {dto.ClienteId} no existe.");
                }

                bool usuarioExiste = await _context.Usuarios.AnyAsync(u => u.UsuarioId == dto.UsuarioId);
                if (!usuarioExiste)
                {
                    throw new RecursoNoExisteException($"El usuario responsable con ID {dto.UsuarioId} no existe.");
                }

                bool estadoExiste = await _context.EstadosProyecto.AnyAsync(e => e.EstadoId == dto.EstadoId);
                if (!estadoExiste)
                {
                    throw new RecursoNoExisteException($"El estado de proyecto con ID {dto.EstadoId} no existe.");
                }

                // Buscamos por defecto el estado de taller "En espera" (o puedes ajustar según tu lógica de negocio)
                var estadoTallerEnEspera = await _context.EstadosTaller
                    .FirstOrDefaultAsync(e => e.NombreEstadoTaller == "En espera");

                int estadoTallerIdDefault = estadoTallerEnEspera != null ? estadoTallerEnEspera.EstadoTallerId : 1;

                Proyecto nuevoProyecto = new Proyecto
                {
                    ClienteId = dto.ClienteId,
                    UsuarioId = dto.UsuarioId,
                    NombreProyecto = dto.NombreProyecto,
                    EstadoId = dto.EstadoId,
                    EstadoTallerId = estadoTallerIdDefault, // Asignamos el ID correspondiente
                    Observaciones = dto.Observaciones,
                    FechaCreacion = DateTime.Now
                };

                _context.Proyectos.Add(nuevoProyecto);
                await _context.SaveChangesAsync();

                HistorialProyecto historial = new HistorialProyecto
                {
                    ProyectoId = nuevoProyecto.ProyectoId,
                    EstadoId = dto.EstadoId,
                    FechaCambio = DateTime.Now
                };
                _context.HistorialProyectos.Add(historial);
                await _context.SaveChangesAsync();

                await _context.Entry(nuevoProyecto).Reference(p => p.Cliente!).LoadAsync();
                await _context.Entry(nuevoProyecto).Reference(p => p.Usuario!).LoadAsync();
                await _context.Entry(nuevoProyecto).Reference(p => p.EstadoProyecto!).LoadAsync();
                await _context.Entry(nuevoProyecto).Reference(p => p.EstadoTaller!).LoadAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se creó el proyecto '{dto.NombreProyecto}' (ID {nuevoProyecto.ProyectoId})",
                    modulo: "Proyectos"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Proyecto Creado",
                    mensaje: $"Creaste el proyecto '{dto.NombreProyecto}' exitosamente."
                );

                return new ProyectoListadoResponse(
                    nuevoProyecto.ProyectoId,
                    nuevoProyecto.NombreProyecto,
                    nuevoProyecto.Cliente?.NombreApellido ?? "",
                    nuevoProyecto.Usuario != null ? $"{nuevoProyecto.Usuario.Nombre} {nuevoProyecto.Usuario.Apellido}" : "",
                    nuevoProyecto.EstadoProyecto?.NombreEstado ?? "",
                    nuevoProyecto.EstadoTaller?.NombreEstadoTaller ?? "",
                    nuevoProyecto.FechaCreacion);
            }
            catch (RecursoNoExisteException) { throw; }
            catch (DbUpdateException ex)
            {
                string mensajeInner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new BaseDeDatosException($"Error exacto de Base de Datos: {mensajeInner}");
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al crear el proyecto: {ex.Message}");
            }
        }

        public async Task Actualizar(int id, ActualizarProyectoRequest dto)
        {
            try
            {
                var proyecto = await _context.Proyectos.FindAsync(id);

                if (proyecto == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el proyecto con ID {id}.");
                }

                if (proyecto.EstadoId != dto.EstadoId)
                {
                    bool estadoExiste = await _context.EstadosProyecto.AnyAsync(e => e.EstadoId == dto.EstadoId);
                    if (!estadoExiste)
                    {
                        throw new RecursoNoExisteException($"El nuevo estado con ID {dto.EstadoId} no existe.");
                    }

                    HistorialProyecto historial = new HistorialProyecto
                    {
                        ProyectoId = id,
                        EstadoId = dto.EstadoId,
                        FechaCambio = DateTime.Now
                    };
                    _context.HistorialProyectos.Add(historial);
                    proyecto.EstadoId = dto.EstadoId;
                }

                proyecto.NombreProyecto = dto.NombreProyecto;
                proyecto.ClienteId = dto.ClienteId;
                proyecto.UsuarioId = dto.UsuarioId;

                // Si en tu DTO de actualización recibes el ID del estado de taller, lo asignas directamente:
                // proyecto.EstadoTallerId = dto.EstadoTallerId; 

                proyecto.Observaciones = dto.Observaciones;

                _context.Proyectos.Update(proyecto);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se actualizó el proyecto ID {id} ('{dto.NombreProyecto}')",
                    modulo: "Proyectos"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Proyecto Actualizado",
                    mensaje: $"Actualizaste el proyecto '{dto.NombreProyecto}'."
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
                throw new BaseDeDatosException($"Error al actualizar el proyecto: {ex.Message}");
            }
        }

        public async Task Eliminar(int id)
        {
            try
            {
                var proyecto = await _context.Proyectos
                    .Include(p => p.Ambientes)
                    .FirstOrDefaultAsync(p => p.ProyectoId == id);

                if (proyecto == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el proyecto con ID {id}.");
                }

                string nombreProyecto = proyecto.NombreProyecto;

                _context.Proyectos.Remove(proyecto);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se eliminó el proyecto '{nombreProyecto}' (ID {id})",
                    modulo: "Proyectos"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Proyecto Eliminado",
                    mensaje: $"Eliminaste el proyecto '{nombreProyecto}'."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (DbUpdateException ex)
            {
                string mensajeInner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new BaseDeDatosException($"Error de base de datos al eliminar: {mensajeInner}");
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al eliminar el proyecto: {ex.Message}");
            }
        }
    }
}