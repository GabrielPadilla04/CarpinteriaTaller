namespace FrontendCarpinteriaWeb.DTOs.Usuario.Output
{
    public record UsuarioDtoOutput(
        int UsuarioId,
        string Nombre,
        string Apellido,
        string Email,
        string NombreRoles
    );
}