namespace TP5_Servicios_API_REST.DTOs.VisitaEntrega.Response
{
    public record VisitaEntregaResponse(
        int VisitaId,
        int ProyectoId,
        string? NombreProyecto,
        int UsuarioId,
        string? NombreEncargado,
        string TipoEvento,
        DateOnly Fecha,
        TimeOnly HoraInicio,
        TimeOnly HoraFin
    );
}