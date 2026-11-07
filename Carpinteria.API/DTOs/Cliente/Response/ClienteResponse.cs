namespace TP5_Servicios_API_REST.DTOs.Cliente.Response
{
    public record ClienteResponse(
        int ClienteId,
        string NombreApellido,
        string Telefono,
        string DireccionObra
    );
}