namespace TP5_Servicios_API_REST.DTOs.Auth
{
    public class ResetPasswordDto
    {
        public string Email { get; set; } = string.Empty;
        public string CodigoOtp { get; set; } = string.Empty;
        public string NuevaPassword { get; set; } = string.Empty;
    }
}