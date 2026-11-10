using System.ComponentModel.DataAnnotations;

namespace FrontendCarpinteriaWeb.DTOs.EstadoProyecto.Request
{
    public class CrearEstadoProyectoRequest
    {
        [Required(ErrorMessage = "El nombre del estado es obligatorio.")]
        public string NombreEstado { get; set; } = string.Empty;

        [Range(0, 100, ErrorMessage = "El porcentaje de progreso debe estar entre 0 y 100.")]
        public int PorcentajeProgreso { get; set; }
    }
}