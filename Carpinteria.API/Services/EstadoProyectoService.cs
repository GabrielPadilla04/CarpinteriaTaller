using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.DTOs.EstadoProyecto.Request;
using TP5_Servicios_API_REST.DTOs.EstadoProyecto.Response;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Models;

namespace TP5_Servicios_API_REST.Services
{
    public class EstadoProyectoService
    {
        private readonly AppDbContext _context;
        private readonly BitacoraService _bitacoraService;
        private readonly NotificacionService _notificacionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EstadoProyectoService(
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

        public async Task<List<EstadoProyectoResponse>> ObtenerTodos()
        {
            try
            {
                return await _context.EstadosProyecto
                    .Select(e => new EstadoProyectoResponse(
                        e.EstadoId,
                        e.NombreEstado,
                        e.PorcentajeProgreso))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener los estados de proyecto: {ex.Message}");
            }
        }

        public async Task<EstadoProyectoResponse> ObtenerPorId(int id)
        {
            try
            {
                var estado = await _context.EstadosProyecto.FindAsync(id);
                if (estado == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el estado de proyecto con ID {id}.");
                }

                return new EstadoProyectoResponse(
                    estado.EstadoId,
                    estado.NombreEstado,
                    estado.PorcentajeProgreso);
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener el estado de proyecto: {ex.Message}");
            }
        }

        public async Task<EstadoProyectoResponse> Crear(CrearEstadoProyectoRequest dto)
        {
            try
            {
                bool existe = await _context.EstadosProyecto
                    .AnyAsync(e => e.NombreEstado.ToLower() == dto.NombreEstado.ToLower());

                if (existe)
                {
                    throw new RecursoExistenteException($"Ya existe un estado con el nombre '{dto.NombreEstado}'.");
                }

                var nuevoEstado = new EstadoProyecto
                {
                    NombreEstado = dto.NombreEstado,
                    PorcentajeProgreso = dto.PorcentajeProgreso
                };

                _context.EstadosProyecto.Add(nuevoEstado);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se creó el estado de proyecto '{dto.NombreEstado}' con {dto.PorcentajeProgreso}% de progreso",
                    modulo: "EstadosProyecto"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Estado de Proyecto Creado",
                    mensaje: $"Creaste el estado de proyecto '{dto.NombreEstado}'."
                );

                return new EstadoProyectoResponse(
                    nuevoEstado.EstadoId,
                    nuevoEstado.NombreEstado,
                    nuevoEstado.PorcentajeProgreso);
            }
            catch (RecursoExistenteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al crear el estado de proyecto: {ex.Message}");
            }
        }

        public async Task Actualizar(int id, ActualizarEstadoProyectoRequest dto)
        {
            try
            {
                var estado = await _context.EstadosProyecto.FindAsync(id);
                if (estado == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el estado de proyecto con ID {id}.");
                }

                bool nombreOcupado = await _context.EstadosProyecto
                    .AnyAsync(e => e.NombreEstado.ToLower() == dto.NombreEstado.ToLower() && e.EstadoId != id);

                if (nombreOcupado)
                {
                    throw new RecursoExistenteException($"Ya existe otro estado con el nombre '{dto.NombreEstado}'.");
                }

                estado.NombreEstado = dto.NombreEstado;
                estado.PorcentajeProgreso = dto.PorcentajeProgreso;

                _context.EstadosProyecto.Update(estado);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se actualizó el estado de proyecto ID {id} a '{dto.NombreEstado}' ({dto.PorcentajeProgreso}%)",
                    modulo: "EstadosProyecto"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Estado Actualizado",
                    mensaje: $"Actualizaste el estado de proyecto a '{dto.NombreEstado}'."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (RecursoExistenteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al actualizar el estado de proyecto: {ex.Message}");
            }
        }

        public async Task Eliminar(int id)
        {
            try
            {
                var estado = await _context.EstadosProyecto.FindAsync(id);
                if (estado == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el estado de proyecto con ID {id}.");
                }

                bool tieneProyectos = await _context.Proyectos.AnyAsync(p => p.EstadoId == id);
                if (tieneProyectos)
                {
                    throw new DatosLlegaronErradosException($"No se puede eliminar el estado '{estado.NombreEstado}' porque hay proyectos asociados a él.");
                }

                string nombreEstado = estado.NombreEstado;

                _context.EstadosProyecto.Remove(estado);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se eliminó el estado de proyecto '{nombreEstado}' (ID {id})",
                    modulo: "EstadosProyecto"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Estado Eliminado",
                    mensaje: $"Eliminaste el estado de proyecto '{nombreEstado}'."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (DatosLlegaronErradosException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al eliminar el estado de proyecto: {ex.Message}");
            }
        }
    }
}