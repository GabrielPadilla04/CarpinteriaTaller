using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.DTOs.Medicion.Request;
using TP5_Servicios_API_REST.DTOs.Medicion.Response;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Models;

namespace TP5_Servicios_API_REST.Services
{
    public class MedicionService
    {
        private readonly AppDbContext _context;
        private readonly BitacoraService _bitacoraService;
        private readonly NotificacionService _notificacionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MedicionService(
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

        public async Task<List<MedicionResponse>> ObtenerTodas()
        {
            try
            {
                return await _context.Mediciones
                    .Select(m => new MedicionResponse(
                        m.MedicionId,
                        m.AmbienteId,
                        m.EtiquetaModulo,
                        m.AltoMm,
                        m.AnchoMm,
                        m.ProfundidadMm,
                        m.UbicacionModulo,
                        m.NotesModulo))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener las mediciones: {ex.Message}");
            }
        }

        public async Task<List<MedicionResponse>> ObtenerPorAmbiente(int ambienteId, int page = 1, int pageSize = 10)
        {
            try
            {
                return await _context.Mediciones
                    .Where(m => m.AmbienteId == ambienteId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(m => new MedicionResponse(
                        m.MedicionId,
                        m.AmbienteId,
                        m.EtiquetaModulo,
                        m.AltoMm,
                        m.AnchoMm,
                        m.ProfundidadMm,
                        m.UbicacionModulo,
                        m.NotesModulo))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener las mediciones: {ex.Message}");
            }
        }

        public async Task<MedicionResponse> ObtenerPorId(int id)
        {
            try
            {
                var m = await _context.Mediciones.FindAsync(id);
                if (m == null)
                {
                    throw new RecursoNoExisteException($"No se encontró la medición con ID {id}.");
                }

                return new MedicionResponse(
                    m.MedicionId,
                    m.AmbienteId,
                    m.EtiquetaModulo,
                    m.AltoMm,
                    m.AnchoMm,
                    m.ProfundidadMm,
                    m.UbicacionModulo,
                    m.NotesModulo);
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener la medición: {ex.Message}");
            }
        }

        public async Task<MedicionResponse> Crear(CrearMedicionRequest dto)
        {
            try
            {
                var ambienteExiste = await _context.Ambientes.AnyAsync(a => a.AmbienteId == dto.AmbienteId);
                if (!ambienteExiste)
                {
                    throw new RecursoNoExisteException("El ambiente especificado no existe.");
                }

                var nuevaMedicion = new Medicion
                {
                    AmbienteId = dto.AmbienteId,
                    EtiquetaModulo = dto.EtiquetaModulo,
                    AltoMm = dto.AltoMm,
                    AnchoMm = dto.AnchoMm,
                    ProfundidadMm = dto.ProfundidadMm,
                    UbicacionModulo = dto.UbicacionModulo,
                    NotesModulo = dto.NotesModulo
                };

                _context.Mediciones.Add(nuevaMedicion);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se registró la medición '{dto.EtiquetaModulo}' (ID {nuevaMedicion.MedicionId}) para el ambiente {dto.AmbienteId}",
                    modulo: "Mediciones"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Medición Registrada",
                    mensaje: $"Registraste la medición '{dto.EtiquetaModulo}' en el ambiente {dto.AmbienteId}."
                );

                return new MedicionResponse(
                    nuevaMedicion.MedicionId,
                    nuevaMedicion.AmbienteId,
                    nuevaMedicion.EtiquetaModulo,
                    nuevaMedicion.AltoMm,
                    nuevaMedicion.AnchoMm,
                    nuevaMedicion.ProfundidadMm,
                    nuevaMedicion.UbicacionModulo,
                    nuevaMedicion.NotesModulo);
            }
            catch (Exception ex) when (ex is not RecursoNoExisteException)
            {
                throw new BaseDeDatosException($"Error al registrar la medición: {ex.Message}");
            }
        }

        public async Task Actualizar(int id, ActualizarMedicionRequest dto)
        {
            try
            {
                var medicion = await _context.Mediciones.FindAsync(id);
                if (medicion == null)
                {
                    throw new RecursoNoExisteException($"No se encontró la medición con ID {id}.");
                }

                var ambienteExiste = await _context.Ambientes.AnyAsync(a => a.AmbienteId == dto.AmbienteId);
                if (!ambienteExiste)
                {
                    throw new RecursoNoExisteException("El ambiente especificado no existe.");
                }

                medicion.AmbienteId = dto.AmbienteId;
                medicion.EtiquetaModulo = dto.EtiquetaModulo;
                medicion.AltoMm = dto.AltoMm;
                medicion.AnchoMm = dto.AnchoMm;
                medicion.ProfundidadMm = dto.ProfundidadMm;
                medicion.UbicacionModulo = dto.UbicacionModulo;
                medicion.NotesModulo = dto.NotesModulo;

                _context.Mediciones.Update(medicion);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se actualizó la medición ID {id} ('{dto.EtiquetaModulo}')",
                    modulo: "Mediciones"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Medición Actualizada",
                    mensaje: $"Actualizaste la medición '{dto.EtiquetaModulo}'."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al actualizar la medición: {ex.Message}");
            }
        }

        public async Task Eliminar(int id)
        {
            try
            {
                var medicion = await _context.Mediciones.FindAsync(id);
                if (medicion == null)
                {
                    throw new RecursoNoExisteException($"No se encontró la medición con ID {id}.");
                }

                string etiquetaModulo = medicion.EtiquetaModulo;

                _context.Mediciones.Remove(medicion);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se eliminó la medición '{etiquetaModulo}' (ID {id})",
                    modulo: "Mediciones"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Medición Eliminada",
                    mensaje: $"Eliminaste la medición '{etiquetaModulo}'."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al eliminar la medición: {ex.Message}");
            }
        }
    }
}