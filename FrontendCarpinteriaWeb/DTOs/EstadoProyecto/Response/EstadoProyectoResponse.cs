namespace FrontendCarpinteriaWeb.DTOs.EstadoProyecto.Response
{
    public record EstadoProyectoResponse
    (
        int EstadoId,
        string NombreEstado,
        int PorcentajeProgreso
    );
}