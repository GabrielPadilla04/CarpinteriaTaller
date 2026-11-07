namespace TP5_Servicios_API_REST.DTOs.Auth
{
    public class RegisterDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int RolesId { get; set; } = 2;
    }
}