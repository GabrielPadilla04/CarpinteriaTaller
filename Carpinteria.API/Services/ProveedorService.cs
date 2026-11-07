using Microsoft.EntityFrameworkCore;
using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Models;

namespace TP5_Servicios_API_REST.Services
{
    public class ProveedorService
    {
        private readonly AppDbContext _context;

        public ProveedorService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Proveedor>> ObtenerTodos()
        {
            try
            {
                // Opcional: Filtrar solo los proveedores activos (borrado lógico)
                return await _context.Proveedores.Where(p => p.Estado).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener los proveedores: {ex.Message}");
            }
        }

        public async Task<Proveedor> ObtenerPorId(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null || !proveedor.Estado)
            {
                throw new RecursoNoExisteException($"No se encontró el proveedor con ID {id}.");
            }
            return proveedor;
        }

        public async Task<Proveedor> Crear(Proveedor proveedor)
        {
            try
            {
                bool existeCuit = await _context.Proveedores.AnyAsync(p => p.Cuit == proveedor.Cuit);
                if (existeCuit)
                {
                    throw new BaseDeDatosException($"Ya existe un proveedor registrado con el CUIT {proveedor.Cuit}.");
                }

                proveedor.Estado = true;
                _context.Proveedores.Add(proveedor);
                await _context.SaveChangesAsync();
                return proveedor;
            }
            catch (DbUpdateException ex)
            {
                string mensajeInner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new BaseDeDatosException($"Error de base de datos al crear el proveedor: {mensajeInner}");
            }
        }

        public async Task<bool> Actualizar(int id, Proveedor proveedorDto)
        {
            try
            {
                var proveedorExistente = await _context.Proveedores.FindAsync(id);
                if (proveedorExistente == null)
                {
                    return false;
                }

                // Validar CUIT duplicado si se está modificando el CUIT
                if (proveedorExistente.Cuit != proveedorDto.Cuit)
                {
                    bool existeCuit = await _context.Proveedores.AnyAsync(p => p.Cuit == proveedorDto.Cuit);
                    if (existeCuit)
                    {
                        throw new BaseDeDatosException($"Ya existe otro proveedor registrado con el CUIT {proveedorDto.Cuit}.");
                    }
                }

                proveedorExistente.RazonSocial = proveedorDto.RazonSocial;
                proveedorExistente.Cuit = proveedorDto.Cuit;
                proveedorExistente.NombreContacto = proveedorDto.NombreContacto;
                proveedorExistente.Telefono = proveedorDto.Telefono;
                proveedorExistente.Email = proveedorDto.Email;
                proveedorExistente.Direccion = proveedorDto.Direccion;
                proveedorExistente.Localidad = proveedorDto.Localidad;
                proveedorExistente.Provincia = proveedorDto.Provincia;
                proveedorExistente.TipoProveedor = proveedorDto.TipoProveedor;
                proveedorExistente.Estado = proveedorDto.Estado;

                _context.Proveedores.Update(proveedorExistente);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                string mensajeInner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new BaseDeDatosException($"Error de base de datos al actualizar el proveedor: {mensajeInner}");
            }
        }

        public async Task<bool> EliminarLogico(int id)
        {
            try
            {
                var proveedor = await _context.Proveedores.FindAsync(id);
                if (proveedor == null || !proveedor.Estado)
                {
                    return false;
                }

                // Borrado lógico: cambiamos el estado a false en lugar de eliminar el registro
                proveedor.Estado = false;
                _context.Proveedores.Update(proveedor);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                string mensajeInner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new BaseDeDatosException($"Error al realizar el borrado lógico del proveedor: {mensajeInner}");
            }
        }
    }
}