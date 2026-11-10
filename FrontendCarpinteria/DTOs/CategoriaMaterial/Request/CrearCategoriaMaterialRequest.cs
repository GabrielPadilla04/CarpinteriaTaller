using System.ComponentModel.DataAnnotations;

namespace FrontendCarpinteria.DTOs.CategoriaMaterial.Request
{
    public class CrearCategoriaMaterialRequest
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        public string NombreCategoria { get; set; } = string.Empty;
    }
}