using System.ComponentModel.DataAnnotations;

namespace TP5_Servicios_API_REST.DTOs.Auth
{
    public class LoginDto
    {
        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        public string Password { get; set; } = null!;
    }
}