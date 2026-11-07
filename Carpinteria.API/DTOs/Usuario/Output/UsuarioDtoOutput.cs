namespace TP5_Servicios_API_REST.DTOs.Usuario.Output
{
    public record UsuarioDtoOutput(
        int UsuarioId,
        string Nombre,
        string Apellido,
        string Email,
        string NombreRoles
    );
}