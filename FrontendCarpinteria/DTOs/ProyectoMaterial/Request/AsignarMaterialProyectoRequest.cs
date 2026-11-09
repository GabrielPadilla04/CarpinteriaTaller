using System.ComponentModel.DataAnnotations;

namespace FrontendCarpinteria.DTOs.ProyectoMaterial.Request
{
    public class AsignarMaterialProyectoRequest
    {
        [Required(ErrorMessage = "El ID del proyecto es obligatorio.")]
        public int ProyectoId { get; set; }

        [Required(ErrorMessage = "El ID del material es obligatorio.")]
        public int MaterialId { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Cantidad { get; set; }
    }
}