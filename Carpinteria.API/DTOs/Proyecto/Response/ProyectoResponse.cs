namespace TP5_Servicios_API_REST.DTOs.Proyecto.Response
{
    public record ProyectoResponse(
        int ProyectoId,
        int ClienteId,
        string? NombreCliente,
        int UsuarioId,
        string? NombreResponsable,
        string NombreProyecto,
        int EstadoId,
        string? NombreEstado,
        string EstadoTaller,
        string? Observaciones,
        DateTime FechaCreacion
    );
}