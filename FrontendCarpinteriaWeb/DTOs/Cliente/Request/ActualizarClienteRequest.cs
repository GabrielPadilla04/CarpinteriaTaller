using System.ComponentModel.DataAnnotations;

namespace FrontendCarpinteriaWeb.DTOs.Cliente.Request
{
    public class ActualizarClienteRequest
    {
        [Required(ErrorMessage = "El nombre y apellido son obligatorios.")]
        public string NombreApellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio.")]
        public string Telefono { get; set; } = string.Empty;

        [Required(ErrorMessage = "La dirección de la obra es obligatoria.")]
        public string DireccionObra { get; set; } = string.Empty;
    }
}