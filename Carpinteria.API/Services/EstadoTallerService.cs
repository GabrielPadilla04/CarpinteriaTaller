using Microsoft.EntityFrameworkCore;
using TP5_Servicios_API_REST.Data;
using TP5_Servicios_API_REST.Excepciones;
using TP5_Servicios_API_REST.Models;

namespace TP5_Servicios_API_REST.Services
{
    public class EstadoTallerService
    {
        private readonly AppDbContext _context;

        public EstadoTallerService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<EstadoTaller>> ObtenerTodos()
        {
            try
            {
                return await _context.EstadosTaller.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new BaseDeDatosException($"Error al obtener los estados de taller: {ex.Message}");
            }
        }

        public async Task<EstadoTaller> ObtenerPorId(int id)
        {
            var estado = await _context.EstadosTaller.FindAsync(id);
            if (estado == null)
            {
                throw new RecursoNoExisteException($"No se encontró el estado de taller con ID {id}.");
            }
            return estado;
        }
    }
}