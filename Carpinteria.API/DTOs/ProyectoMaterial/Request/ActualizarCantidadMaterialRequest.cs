using System.ComponentModel.DataAnnotations;

namespace TP5_Servicios_API_REST.DTOs.ProyectoMaterial.Request
{
    public class ActualizarCantidadMaterialRequest
    {
        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public int Cantidad { get; set; }
    }
}