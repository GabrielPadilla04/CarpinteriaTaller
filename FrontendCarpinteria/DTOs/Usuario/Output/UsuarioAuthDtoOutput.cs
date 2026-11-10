namespace FrontendCarpinteria.DTOs.Usuario.Output
{
    // DTO Específico que se suele devolver al actualizar o al hacer login
    public class UsuarioAuthDtoOutput
    {
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; }

        public UsuarioAuthDtoOutput(int usuarioId, string nombre, string apellido, string email, string rol)
        {
            UsuarioId = usuarioId;
            Nombre = nombre;
            Apellido = apellido;
            Email = email;
            Rol = rol;
        }
    }
}