using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TP5_Servicios_API_REST.DTOs.CategoriaMaterial.Request;
using TP5_Servicios_API_REST.DTOs.CategoriaMaterial.Response;

namespace TP5_Servicios_API_REST.Services
{
    public class CategoriaMaterialService
    {
        private readonly AppDbContext _context;
        private readonly BitacoraService _bitacoraService;
        private readonly NotificacionService _notificacionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CategoriaMaterialService(
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

        public async Task<List<CategoriaMaterialResponse>> ObtenerTodos(int page, int pageSize)
        {
            try
            {
                return await _context.CategoriasMaterial
                    .Skip((page - 1) * pageSize) // Corregido el parámetro de paginación
                    .Take(pageSize)
                    .Select(c => new CategoriaMaterialResponse(c.CategoriaId, c.NombreCategoria))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener las categorías de materiales: {ex.Message}");
            }
        }

        public async Task<CategoriaMaterialResponse> ObtenerPorId(int id)
        {
            try
            {
                var categoria = await _context.CategoriasMaterial.FindAsync(id);

                if (categoria == null)
                {
                    throw new RecursoNoExisteException($"No se encontró la categoría de material con ID {id}.");
                }

                return new CategoriaMaterialResponse(categoria.CategoriaId, categoria.NombreCategoria);
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener la categoría de material: {ex.Message}");
            }
        }

        public async Task<CategoriaMaterialResponse> Crear(CrearCategoriaMaterialRequest dto)
        {
            try
            {
                bool existe = await _context.CategoriasMaterial
                    .AnyAsync(c => c.NombreCategoria.ToLower() == dto.NombreCategoria.ToLower());

                if (existe)
                {
                    throw new RecursoExistenteException($"Ya existe una categoría con el nombre '{dto.NombreCategoria}'.");
                }

                CategoriaMaterial nuevaCategoria = new CategoriaMaterial
                {
                    NombreCategoria = dto.NombreCategoria
                };

                _context.CategoriasMaterial.Add(nuevaCategoria);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se creó la categoría de material '{dto.NombreCategoria}' (ID {nuevaCategoria.CategoriaId})",
                    modulo: "CategoriasMaterial"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Categoría Creada",
                    mensaje: $"Creaste la categoría de material '{dto.NombreCategoria}'."
                );

                return new CategoriaMaterialResponse(nuevaCategoria.CategoriaId, nuevaCategoria.NombreCategoria);
            }
            catch (RecursoExistenteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al crear la categoría de material: {ex.Message}");
            }
        }

        public async Task Actualizar(int id, ActualizarCategoriaMaterialRequest dto)
        {
            try
            {
                var categoria = await _context.CategoriasMaterial.FindAsync(id);

                if (categoria == null)
                {
                    throw new RecursoNoExisteException($"No se encontró la categoría de material con ID {id}.");
                }

                bool nombreOcupado = await _context.CategoriasMaterial
                    .AnyAsync(c => c.NombreCategoria.ToLower() == dto.NombreCategoria.ToLower() && c.CategoriaId != id);

                if (nombreOcupado)
                {
                    throw new RecursoExistenteException($"Ya existe otra categoría con el nombre '{dto.NombreCategoria}'.");
                }

                categoria.NombreCategoria = dto.NombreCategoria;

                _context.CategoriasMaterial.Update(categoria);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se actualizó la categoría de material ID {id} a '{dto.NombreCategoria}'",
                    modulo: "CategoriasMaterial"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Categoría Actualizada",
                    mensaje: $"Actualizaste la categoría a '{dto.NombreCategoria}'."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (RecursoExistenteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al actualizar la categoría de material: {ex.Message}");
            }
        }

        public async Task Eliminar(int id)
        {
            try
            {
                var categoria = await _context.CategoriasMaterial.FindAsync(id);

                if (categoria == null)
                {
                    throw new RecursoNoExisteException($"No se encontró la categoría de material con ID {id}.");
                }

                // VALIDACIÓN: Verificamos si hay materiales que utilicen esta categoría antes de borrar
                bool tieneMateriales = await _context.Materiales.AnyAsync(m => m.CategoriaId == id);
                if (tieneMateriales)
                {
                    throw new DatosLlegaronErradosException($"No se puede eliminar la categoría '{categoria.NombreCategoria}' porque tiene materiales de catálogo asociados.");
                }

                string nombreCategoria = categoria.NombreCategoria;

                _context.CategoriasMaterial.Remove(categoria);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se eliminó la categoría de material '{nombreCategoria}' (ID {id})",
                    modulo: "CategoriasMaterial"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Categoría Eliminada",
                    mensaje: $"Eliminaste la categoría de material '{nombreCategoria}'."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (DatosLlegaronErradosException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al eliminar la categoría de material: {ex.Message}");
            }
        }
    }
}