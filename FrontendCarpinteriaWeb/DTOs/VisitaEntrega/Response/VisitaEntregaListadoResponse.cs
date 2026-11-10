namespace FrontendCarpinteriaWeb.DTOs.VisitaEntrega.Response
{
    public record VisitaEntregaListadoResponse(
        int VisitaId,
        string NombreProyecto,
        string NombreEncargado,
        string TipoEvento,
        DateOnly Fecha,
        TimeOnly HoraInicio,
        TimeOnly HoraFin
    );
}