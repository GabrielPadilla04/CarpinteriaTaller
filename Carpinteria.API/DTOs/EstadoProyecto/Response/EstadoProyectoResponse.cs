namespace TP5_Servicios_API_REST.DTOs.EstadoProyecto.Response
{
    public record EstadoProyectoResponse
    (
        int EstadoId,
        string NombreEstado,
        int PorcentajeProgreso
    );
}