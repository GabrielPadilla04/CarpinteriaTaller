using System.ComponentModel.DataAnnotations;

namespace FrontendCarpinteria.DTOs.CategoriaMaterial.Request
{
    public class ActualizarCategoriaMaterialRequest
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        public string NombreCategoria { get; set; } = string.Empty;
    }
}