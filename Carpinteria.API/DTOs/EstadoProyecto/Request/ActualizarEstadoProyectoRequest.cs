using System.ComponentModel.DataAnnotations;

namespace TP5_Servicios_API_REST.DTOs.EstadoProyecto.Request
{
    public class ActualizarEstadoProyectoRequest
    {
        [Required(ErrorMessage = "El nombre del estado es obligatorio.")]
        public string NombreEstado { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "El porcentaje de progreso debe estar entre 0 y 100.")]
        public int PorcentajeProgreso { get; set; }
    }
}