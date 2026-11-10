namespace FrontendCarpinteriaWeb.DTOs.Ambiente.Response
{
    public record AmbienteResponse(
        int AmbienteId,
        int ProyectoId,
        string NombreAmbiente
    );
}