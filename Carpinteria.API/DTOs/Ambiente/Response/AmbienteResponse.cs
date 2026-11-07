namespace TP5_Servicios_API_REST.DTOs.Ambiente.Response
{
    public record AmbienteResponse(
        int AmbienteId,
        int ProyectoId,
        string NombreAmbiente
    );
}