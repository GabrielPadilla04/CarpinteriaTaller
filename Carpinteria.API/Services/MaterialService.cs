using Carpinteria.API.DTOs.Material.Response;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.DTOs.Material.Request;
using Carpinteria.API.DTOs.Material.Response;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Models;

namespace TP5_Servicios_API_REST.Services
{
    public class MaterialService
    {
        private readonly AppDbContext _context;
        private readonly BitacoraService _bitacoraService;
        private readonly NotificacionService _notificacionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public MaterialService(
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

        public async Task<List<MaterialListadoResponse>> ObtenerTodos(int page = 1, int pageSize = 10)
        {
            try
            {
                return await _context.Materiales
                    .Include(m => m.CategoriaMaterial)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(m => new MaterialListadoResponse(
                        m.MaterialId,
                        m.CategoriaId, // <--- Incluido para resolver el filtrado por ID si se requiere
                        m.MarcaProveedor,
                        m.NombreColorDiseno,
                        m.Acabado,
                        m.EspesorDisponible,
                        m.CategoriaMaterial != null ? m.CategoriaMaterial.NombreCategoria : "Sin categoría",
                        m.ImagenUrl)) // <--- Mapeo de la imagen
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener los materiales: {ex.Message}");
            }
        }

        public async Task<MaterialResponse> ObtenerPorId(int id)
        {
            try
            {
                var material = await _context.Materiales
                    .Include(m => m.CategoriaMaterial)
                    .FirstOrDefaultAsync(m => m.MaterialId == id);

                if (material == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el material con ID {id}.");
                }

                return new MaterialResponse(
                    material.MaterialId,
                    material.CategoriaId,
                    material.CategoriaMaterial?.NombreCategoria,
                    material.MarcaProveedor,
                    material.NombreColorDiseno,
                    material.Acabado,
                    material.EspesorDisponible,
                    material.ImagenUrl); // <--- Mapeo de la imagen
            }
            catch (RecursoNoExisteException) { throw; }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener el material: {ex.Message}");
            }
        }

        public async Task<MaterialListadoResponse> Crear(CrearMaterialRequest dto)
        {
            try
            {
                // Validar que la categoría de material asignada realmente exista
                bool categoriaExiste = await _context.CategoriasMaterial.AnyAsync(c => c.CategoriaId == dto.CategoriaId);
                if (!categoriaExiste)
                {
                    throw new RecursoNoExisteException($"La categoría de material con ID {dto.CategoriaId} no existe.");
                }

                Material nuevoMaterial = new Material
                {
                    CategoriaId = dto.CategoriaId,
                    MarcaProveedor = dto.MarcaProveedor,
                    NombreColorDiseno = dto.NombreColorDiseno,
                    Acabado = dto.Acabado,
                    EspesorDisponible = dto.EspesorDisponible,
                    ImagenUrl = dto.ImagenUrl // <--- Asignación de la imagen
                };

                _context.Materiales.Add(nuevoMaterial);
                await _context.SaveChangesAsync();

                // Cargar la relación para devolver el nombre de la categoría
                await _context.Entry(nuevoMaterial).Reference(m => m.CategoriaMaterial!).LoadAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se creó el material '{dto.NombreColorDiseno}' de {dto.MarcaProveedor} (ID {nuevoMaterial.MaterialId})",
                    modulo: "Materiales"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Material Creado",
                    mensaje: $"Creaste el material '{dto.NombreColorDiseno}' de {dto.MarcaProveedor}."
                );

                return new MaterialListadoResponse(
                    nuevoMaterial.MaterialId,
                    nuevoMaterial.CategoriaId,
                    nuevoMaterial.MarcaProveedor,
                    nuevoMaterial.NombreColorDiseno,
                    nuevoMaterial.Acabado,
                    nuevoMaterial.EspesorDisponible,
                    nuevoMaterial.CategoriaMaterial?.NombreCategoria,
                    nuevoMaterial.ImagenUrl);
            }
            catch (RecursoNoExisteException) { throw; }
            catch (DbUpdateException ex)
            {
                string mensajeInner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new BaseDeDatosException($"Error exacto de Base de Datos: {mensajeInner}");
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al crear el material: {ex.Message}");
            }
        }

        public async Task Actualizar(int id, ActualizarMaterialRequest dto)
        {
            try
            {
                var material = await _context.Materiales.FindAsync(id);

                if (material == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el material con ID {id}.");
                }

                if (material.CategoriaId != dto.CategoriaId)
                {
                    bool categoriaExiste = await _context.CategoriasMaterial.AnyAsync(c => c.CategoriaId == dto.CategoriaId);
                    if (!categoriaExiste)
                    {
                        throw new RecursoNoExisteException($"La nueva categoría con ID {dto.CategoriaId} no existe.");
                    }
                }

                material.CategoriaId = dto.CategoriaId;
                material.MarcaProveedor = dto.MarcaProveedor;
                material.NombreColorDiseno = dto.NombreColorDiseno;
                material.Acabado = dto.Acabado;
                material.EspesorDisponible = dto.EspesorDisponible;
                material.ImagenUrl = dto.ImagenUrl; // <--- Actualización de la imagen

                _context.Materiales.Update(material);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se actualizó el material ID {id} ('{dto.NombreColorDiseno}')",
                    modulo: "Materiales"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Material Actualizado",
                    mensaje: $"Actualizaste el material '{dto.NombreColorDiseno}'."
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
                throw new BaseDeDatosException($"Error al actualizar el material: {ex.Message}");
            }
        }

        public async Task Eliminar(int id)
        {
            try
            {
                var material = await _context.Materiales.FindAsync(id);

                if (material == null)
                {
                    throw new RecursoNoExisteException($"No se encontró el material con ID {id}.");
                }

                // VALIDACIÓN: Verificar si el material está siendo utilizado en proyectos activos (tabla intermedia proyectos_materiales)
                bool estaEnProyectos = await _context.ProyectosMateriales.AnyAsync(pm => pm.MaterialId == id);
                if (estaEnProyectos)
                {
                    throw new DatosLlegaronErradosException($"No se puede eliminar el material '{material.NombreColorDiseno}' porque está asociado a uno o más proyectos de carpintería.");
                }

                string nombreMaterial = material.NombreColorDiseno;

                _context.Materiales.Remove(material);
                await _context.SaveChangesAsync();

                // Registrar Bitácora y Notificación
                int usuarioIdActual = ObtenerUsuarioIdActual();

                await _bitacoraService.RegistrarAccionAsync(
                    usuarioId: usuarioIdActual,
                    accion: $"Se eliminó el material '{nombreMaterial}' (ID {id})",
                    modulo: "Materiales"
                );

                await _notificacionService.EnviarNotificacionAsync(
                    usuarioId: usuarioIdActual,
                    titulo: "Material Eliminado",
                    mensaje: $"Eliminaste el material '{nombreMaterial}'."
                );
            }
            catch (RecursoNoExisteException) { throw; }
            catch (DatosLlegaronErradosException) { throw; }
            catch (DbUpdateException ex)
            {
                string mensajeInner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new BaseDeDatosException($"Error exacto de Base de Datos: {mensajeInner}");
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al eliminar el material: {ex.Message}");
            }
        }
    }
}