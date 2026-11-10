using Microsoft.EntityFrameworkCore;
using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.Excepciones;
using System.Security.Claims;

namespace TP5_Servicios_API_REST.Services
{
    public class DashboardService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DashboardService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int ObtenerUsuarioIdActual()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int usuarioId))
            {
                throw new UnauthorizedAccessException("Usuario no autenticado.");
            }
            return usuarioId;
        }

        public async Task<UsuarioPerfilDto> ObtenerPerfilUsuarioAsync()
        {
            try
            {
                int usuarioId = ObtenerUsuarioIdActual();

                var usuario = await _context.Usuarios
                    .Include(u => u.Roles)
                    .Where(u => u.UsuarioId == usuarioId)
                    .Select(u => new UsuarioPerfilDto
                    {
                        Nombre = $"{u.Nombre} {u.Apellido}".Trim(),
                        Rol = u.Roles != null ? u.Roles.NombreRoles : "Usuario"
                    })
                    .FirstOrDefaultAsync();

                return usuario ?? new UsuarioPerfilDto { Nombre = "Usuario", Rol = "Carpintero / Diseñador" };
            }
            catch (Exception ex) when (ex is not UnauthorizedAccessException)
            {
                throw new BaseDeDatosException("Error al obtener el perfil del usuario.");
            }
        }

        public async Task<List<VisitaDto>> ObtenerVisitasHoyAsync()
        {
            try
            {
                int usuarioId = ObtenerUsuarioIdActual();
                var hoy = DateOnly.FromDateTime(DateTime.Today);

                return await _context.VisitasEntregas
                    .Where(v => v.UsuarioId == usuarioId && v.Fecha == hoy)
                    .Include(v => v.Proyecto)
                    .Select(v => new VisitaDto
                    {
                        Id = v.VisitaId,
                        Titulo = $"{v.TipoEvento} - {(v.Proyecto != null ? v.Proyecto.NombreProyecto : $"Proyecto #{v.ProyectoId}")}",
                        Horario = $"{v.HoraInicio:HH:mm} a {v.HoraFin:HH:mm}"
                    })
                    .ToListAsync();
            }
            catch (Exception ex) when (ex is not UnauthorizedAccessException)
            {
                throw new BaseDeDatosException("Error al obtener las visitas de hoy.");
            }
        }

        public async Task<List<ProyectoActivoDto>> ObtenerProyectosActivosAsync()
        {
            try
            {
                int usuarioId = ObtenerUsuarioIdActual();

                return await _context.Proyectos
                    .Where(p => p.UsuarioId == usuarioId)
                    .Include(p => p.EstadoTaller)
                    .Select(p => new ProyectoActivoDto
                    {
                        Id = p.ProyectoId,
                        Nombre = p.NombreProyecto,
                        Estado = p.EstadoTaller != null ? p.EstadoTaller.NombreEstadoTaller : "En curso",
                        PorcentajeProgreso = 75,
                        FechaEntrega = p.FechaCreacion.AddDays(30).ToString("dd 'de' MMMM"),
                        ComentariosCount = 0
                    })
                    .ToListAsync();
            }
            catch (Exception ex) when (ex is not UnauthorizedAccessException)
            {
                throw new BaseDeDatosException("Error al obtener los proyectos activos.");
            }
        }
    }

    // DTOs de respuesta
    public class UsuarioPerfilDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
    }

    public class VisitaDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Horario { get; set; } = string.Empty;
    }

    public class ProyectoActivoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public int PorcentajeProgreso { get; set; }
        public string FechaEntrega { get; set; } = string.Empty;
        public int ComentariosCount { get; set; }
    }
}