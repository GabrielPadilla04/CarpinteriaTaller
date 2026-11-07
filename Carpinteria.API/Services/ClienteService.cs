using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TP5_Servicios_API_REST.DTOs.Cliente.Request;
using TP5_Servicios_API_REST.DTOs.Cliente.Response;

namespace TP5_Servicios_API_REST.Services
{
    public class ClienteService
    {
        private readonly AppDbContext _context;
        private readonly BitacoraService _bitacoraService;
        private readonly NotificacionService _notificacionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClienteService(
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

        public async Task<List<ClienteResponse>> ObtenerTodos(int page, int pageSize)
        {
            try
            {
                return await _context.Clientes
                    .Skip((page - 1) * pageSize) // Corregido el parámetro de paginación
                    .Take(pageSize)
                    .Select(c => new ClienteResponse(c.ClienteId, c.NombreApellido, c.Telefono, c.DireccionObra))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener los clientes: {ex.Message}");
            }
        }

        public async Task<ClienteResponse> ObtenerPorId(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);

                if (cliente == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el cliente con ID {id}.");
                }

                return new ClienteResponse(cliente.ClienteId, cliente.NombreApellido, cliente.Telefono, cliente.DireccionObra);
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener el cliente: {ex.Message}");
            }
        }

        public async Task<ClienteResponse> Crear(CrearClienteRequest dto)
        {
            try
            {
                Cliente nuevoCliente = new Cliente
                {
                    NombreApellido = dto.NombreApellido,
                    Telefono = dto.Telefono,
                    DireccionObra = dto.DireccionObra
                };

                _context.Clientes.Add(nuevoCliente);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se registró el cliente '{dto.NombreApellido}' (ID {nuevoCliente.ClienteId})",
                    modulo: "Clientes"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Cliente Registrado",
                    mensaje: $"Registraste al cliente '{dto.NombreApellido}' exitosamente."
                );

                return new ClienteResponse(nuevoCliente.ClienteId, nuevoCliente.NombreApellido, nuevoCliente.Telefono, nuevoCliente.DireccionObra);
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al crear el cliente: {ex.Message}");
            }
        }

        public async Task Actualizar(int id, ActualizarClienteRequest dto)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);

                if (cliente == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el cliente con ID {id}.");
                }

                cliente.NombreApellido = dto.NombreApellido;
                cliente.Telefono = dto.Telefono;
                cliente.DireccionObra = dto.DireccionObra;

                _context.Clientes.Update(cliente);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se actualizó la información del cliente ID {id} ('{dto.NombreApellido}')",
                    modulo: "Clientes"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Cliente Actualizado",
                    mensaje: $"Actualizaste los datos del cliente '{dto.NombreApellido}'."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al actualizar el cliente: {ex.Message}");
            }
        }

        public async Task Eliminar(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);

                if (cliente == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el cliente con ID {id}.");
                }

                // VALIDACIÓN: Verificamos si el cliente tiene proyectos de carpintería asociados
                bool tieneProyectos = await _context.Proyectos.AnyAsync(p => p.ClienteId == id);
                if (tieneProyectos)
                {
                    throw new DatosLlegaronErradosException($"No se puede eliminar el cliente '{cliente.NombreApellido}' porque tiene proyectos activos asociados.");
                }

                string nombreCliente = cliente.NombreApellido;

                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se eliminó al cliente '{nombreCliente}' (ID {id})",
                    modulo: "Clientes"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Cliente Eliminado",
                    mensaje: $"Eliminaste al cliente '{nombreCliente}'."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (DatosLlegaronErradosException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al eliminar el cliente: {ex.Message}");
            }
        }
    }
}