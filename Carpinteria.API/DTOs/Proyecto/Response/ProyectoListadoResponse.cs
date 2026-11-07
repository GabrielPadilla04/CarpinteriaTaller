namespace TP5_Servicios_API_REST.DTOs.Proyecto.Response
{
    public record ProyectoListadoResponse(
        int ProyectoId,
        string NombreProyecto,
        string NombreCliente,
        string NombreResponsable,
        string NombreEstado,
        string EstadoTaller,
        DateTime FechaCreacion
    );
}