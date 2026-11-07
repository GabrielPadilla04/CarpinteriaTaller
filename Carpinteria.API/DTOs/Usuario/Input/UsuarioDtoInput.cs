using System.ComponentModel.DataAnnotations;

namespace TP5_Servicios_API_REST.DTOs.Usuario.Input
{
    public class UsuarioDtoInput
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio.")]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido.")]
        public string Email { get; set; } = string.Empty;

        public string? Password { get; set; }

        public int RolesId { get; set; } = 2; // 1: Admin, 2: Carpintero/Diseñador por defecto
    }
}