using Microsoft.EntityFrameworkCore;
using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Models;

namespace TP5_Servicios_API_REST.Services
{
    public class CompraService
    {
        private readonly AppDbContext _context;

        public CompraService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Compra>> ObtenerTodas()
        {
            try
            {
                return await _context.Set<Compra>()
                    .Include(c => c.Proveedor)
                    .Include(c => c.Material)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener el historial de compras: {ex.Message}");
            }
        }

        public async Task<Compra> ObtenerPorId(int id)
        {
            var compra = await _context.Set<Compra>()
                .Include(c => c.Proveedor)
                .Include(c => c.Material)
                .FirstOrDefaultAsync(c => c.CompraId == id);

            if (compra == null)
            {
                throw new RecursoNoExisteException($"No se encontró el registro de compra con ID {id}.");
            }
            return compra;
        }

        public async Task<Compra> Registrar(Compra compra)
        {
            try
            {
                var proveedor = await _context.Proveedores.FindAsync(compra.ProveedorId);
                if (proveedor == null || !proveedor.Estado)
                {
                    throw new BaseDeDatosException("El proveedor especificado no existe o se encuentra inactivo.");
                }

                var material = await _context.Materiales.FindAsync(compra.MaterialId);
                if (material == null)
                {
                    throw new BaseDeDatosException("El material especificado no existe en el catálogo.");
                }

                _context.Set<Compra>().Add(compra);
                await _context.SaveChangesAsync();

                return await ObtenerPorId(compra.CompraId);
            }
            catch (DbUpdateException ex)
            {
                string mensajeInner = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                throw new BaseDeDatosException($"Error de base de datos al registrar la compra: {mensajeInner}");
            }
        }
    }
}