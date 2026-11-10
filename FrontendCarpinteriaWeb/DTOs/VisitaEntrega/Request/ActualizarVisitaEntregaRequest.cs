using System.ComponentModel.DataAnnotations;

namespace FrontendCarpinteriaWeb.DTOs.VisitaEntrega.Request
{
    public class ActualizarVisitaEntregaRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe asociar un proyecto.")]
        public int ProyectoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe asignar un operario encargado.")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El tipo de evento es obligatorio.")]
        public string TipoEvento { get; set; } = string.Empty;

        public DateOnly Fecha { get; set; }
        public TimeOnly HoraInicio { get; set; }
        public TimeOnly HoraFin { get; set; }
    }
}