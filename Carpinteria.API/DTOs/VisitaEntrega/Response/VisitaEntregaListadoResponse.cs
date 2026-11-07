namespace TP5_Servicios_API_REST.DTOs.VisitaEntrega.Response
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