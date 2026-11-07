using System.ComponentModel.DataAnnotations;

namespace TP5_Servicios_API_REST.DTOs.CategoriaMaterial.Request
{
    public class ActualizarCategoriaMaterialRequest
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        public string NombreCategoria { get; set; } = string.Empty;
    }
}