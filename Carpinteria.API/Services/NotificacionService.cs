using Microsoft.EntityFrameworkCore;
using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.DTOs.Notificacion.Response;
using TP5_Servicios_API_REST.Models;
using TP5_Servicios_API_REST.Excepciones;

namespace TP5_Servicios_API_REST.Services
{
    public class NotificacionService
    {
        private readonly AppDbContext _context;

        public NotificacionService(AppDbContext context)
        {
            _context = context;
        }

        // Enviar/Registrar una notificación para un usuario
        public async Task EnviarNotificacionAsync(int usuarioId, string titulo, string mensaje)
        {
            try
            {
                var notificacion = new Notificacion
                {
                    UsuarioId = usuarioId,
                    Titulo = titulo,
                    Mensaje = mensaje,
                    Leido = false,
                    FechaHora = DateTime.Now
                };

                _context.Notificaciones.Add(notificacion);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR AL ENVIAR NOTIFICACION]: {ex.Message}");
            }
        }

        // Obtener las notificaciones de un usuario específico
        public async Task<List<NotificacionResponse>> ObtenerPorUsuarioAsync(int usuarioId)
        {
            try
            {
                return await _context.Notificaciones
                    .Where(n => n.UsuarioId == usuarioId)
                    .OrderByDescending(n => n.FechaHora)
                    .Select(n => new NotificacionResponse(
                        n.NotificacionId,
                        n.UsuarioId,
                        n.Titulo,
                        n.Mensaje,
                        n.Leido,
                        n.FechaHora))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener las notificaciones: {ex.Message}");
            }
        }

        // Marcar notificación como leída
        public async Task MarcarComoLeidaAsync(int notificacionId)
        {
            try
            {
                var notificacion = await _context.Notificaciones.FindAsync(notificacionId);
                if (notificacion == null)
                {
                    throw new RecursoNoExisteException($"No se encontró la notificación con ID {notificacionId}.");
                }

                notificacion.Leido = true;
                _context.Notificaciones.Update(notificacion);
                await _context.SaveChangesAsync();
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al actualizar la notificación: {ex.Message}");
            }
        }
    }
}