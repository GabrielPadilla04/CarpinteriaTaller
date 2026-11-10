namespace FrontendCarpinteriaWeb.DTOs.Cliente.Response
{
    public record ClienteResponse(
        int ClienteId,
        string NombreApellido,
        string Telefono,
        string DireccionObra
    );
}