namespace FrontendCarpinteria.DTOs.Usuario.Output
{
    public record UsuarioDtoOutput(
        int UsuarioId,
        string Nombre,
        string Apellido,
        string Email,
        string NombreRoles
    );
}