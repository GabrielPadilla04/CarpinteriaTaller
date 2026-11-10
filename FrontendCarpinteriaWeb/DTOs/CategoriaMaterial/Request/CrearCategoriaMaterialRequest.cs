using System.ComponentModel.DataAnnotations;

namespace FrontendCarpinteriaWeb.DTOs.CategoriaMaterial.Request
{
    public class CrearCategoriaMaterialRequest
    {
        [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
        public string NombreCategoria { get; set; } = string.Empty;
    }
}