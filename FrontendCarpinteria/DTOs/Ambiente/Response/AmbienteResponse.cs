namespace FrontendCarpinteria.DTOs.Ambiente.Response
{
    public record AmbienteResponse(
        int AmbienteId,
        int ProyectoId,
        string NombreAmbiente
    );
}