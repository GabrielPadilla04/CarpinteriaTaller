using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.DTOs.Usuario.Input;
using TP5_Servicios_API_REST.DTOs.Usuario.Output;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Models;

namespace TP5_Servicios_API_REST.Services
{
    public class UsuarioService
    {
        private readonly AppDbContext _context;
        private readonly BitacoraService _bitacoraService;
        private readonly NotificacionService _notificacionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioService(
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

        public async Task<List<UsuarioDtoOutput>> ObtenerlosATodos(int page = 1, int pageSize = 10)
        {
            try
            {
                return await _context.Usuarios
                    .Include(u => u.Roles)
                    .Skip((page - 1) * pageSize) // Corregido el parámetro de paginación
                    .Take(pageSize)
                    .Select(u => new UsuarioDtoOutput(
                        u.UsuarioId,
                        u.Nombre,
                        u.Apellido,
                        u.Email,
                        u.Roles != null ? u.Roles.NombreRoles : "Sin rol"))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener los usuarios: {ex.Message}");
            }
        }

        public async Task<UsuarioDtoOutput> Crear(UsuarioDtoInput dto)
        {
            try
            {
                bool existeEmail = await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower());
                if (existeEmail)
                {
                    throw new RecursoExistenteException($"Ya existe un usuario con el email '{dto.Email}'.");
                }

                int rolesIdAsignado = dto.RolesId > 0 ? dto.RolesId : 2;

                Usuario nuevoUsuario = new Usuario
                {
                    Nombre = dto.Nombre,
                    Apellido = dto.Apellido,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    RolesId = rolesIdAsignado
                };

                _context.Usuarios.Add(nuevoUsuario);
                await _context.SaveChangesAsync();

                await _context.Entry(nuevoUsuario).Reference(u => u.Roles!).LoadAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se creó el usuario '{dto.Nombre} {dto.Apellido}' (ID {nuevoUsuario.UsuarioId})",
                    modulo: "Usuarios"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Usuario Creado",
                    mensaje: $"Creaste al usuario '{dto.Nombre} {dto.Apellido}' exitosamente."
                );

                return new UsuarioDtoOutput(
                    nuevoUsuario.UsuarioId,
                    nuevoUsuario.Nombre,
                    nuevoUsuario.Apellido,
                    nuevoUsuario.Email,
                    nuevoUsuario.Roles?.NombreRoles ?? "Sin rol");
            }
            catch (RecursoExistenteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al crear el usuario: {ex.Message}");
            }
        }

        public async Task Actualizar(int id, UsuarioDtoInput dto)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);

                if (usuario == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el usuario con ID {id}.");
                }

                bool emailOcupado = await _context.Usuarios
                    .AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower() && u.UsuarioId != id);

                if (emailOcupado)
                {
                    throw new RecursoExistenteException($"El email '{dto.Email}' ya pertenece a otro usuario.");
                }

                usuario.Nombre = dto.Nombre;
                usuario.Apellido = dto.Apellido;
                usuario.Email = dto.Email;

                if (!string.IsNullOrEmpty(dto.Password))
                {
                    usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
                }

                if (dto.RolesId > 0)
                {
                    usuario.RolesId = dto.RolesId;
                }

                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se actualizó la información del usuario ID {id} ('{dto.Nombre} {dto.Apellido}')",
                    modulo: "Usuarios"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Usuario Actualizado",
                    mensaje: $"Actualizaste los datos del usuario '{dto.Nombre} {dto.Apellido}'."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (RecursoExistenteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al actualizar el usuario: {ex.Message}");
            }
        }

        public async Task DarDeBaja(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);

                if (usuario == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el usuario con ID {id}.");
                }

                bool tieneProyectos = await _context.Proyectos.AnyAsync(p => p.UsuarioId == id);
                bool tieneVisitas = await _context.VisitasEntregas.AnyAsync(v => v.UsuarioId == id);

                if (tieneProyectos || tieneVisitas)
                {
                    throw new DatosLlegaronErradosException($"No se puede dar de baja al usuario '{usuario.Nombre} {usuario.Apellido}' porque tiene proyectos activos o visitas asignadas en la agenda.");
                }

                string nombreUsuario = $"{usuario.Nombre} {usuario.Apellido}";

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se dio de baja al usuario '{nombreUsuario}' (ID {id})",
                    modulo: "Usuarios"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Usuario Dado de Baja",
                    mensaje: $"Diste de baja al usuario '{nombreUsuario}'."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (DatosLlegaronErradosException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al dar de baja el usuario: {ex.Message}");
            }
        }

        public async Task AsignarRol(int id, int nuevoRolesId)
        {
            try
            {
                var usuario = await _context.Usuarios
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.UsuarioId == id);

                if (usuario == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el usuario con ID {id}.");
                }

                bool rolesExiste = await _context.Roles.AnyAsync(r => r.RolesId == nuevoRolesId);
                if (!rolesExiste)
                {
                    throw new RecursoNoExisteException($"El rol con ID {nuevoRolesId} no existe.");
                }

                usuario.RolesId = nuevoRolesId;
                _context.Usuarios.Update(usuario);
                await _context.SaveChangesAsync();

                // Recargar el rol para obtener su nombre en la notificación y bitácora
                await _context.Entry(usuario).Reference(u => u.Roles!).LoadAsync();
                string nombreRol = usuario.Roles?.NombreRoles ?? nuevoRolesId.ToString();
                string nombreUsuario = $"{usuario.Nombre} {usuario.Apellido}";

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se asignó el rol '{nombreRol}' al usuario '{nombreUsuario}' (ID {id})",
                    modulo: "Usuarios"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Rol Asignado",
                    mensaje: $"Asignaste el rol '{nombreRol}' al usuario '{nombreUsuario}'."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al cambiar el rol del usuario: {ex.Message}");
            }
        }
    }
}