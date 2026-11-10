using System.ComponentModel.DataAnnotations;

namespace FrontendCarpinteria.DTOs.Proyecto.Request
{
    public class CrearProyectoRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un cliente válido.")]
        public int ClienteId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Debe asignar un operario/diseñador responsable.")]
        public int UsuarioId { get; set; }

        [Required(ErrorMessage = "El nombre del proyecto es obligatorio.")]
        public string NombreProyecto { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar un estado inicial válido.")]
        public int EstadoId { get; set; }

        public string? Observaciones { get; set; }
    }
}