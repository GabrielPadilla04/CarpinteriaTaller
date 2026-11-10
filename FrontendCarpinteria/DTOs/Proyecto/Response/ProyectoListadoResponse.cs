namespace FrontendCarpinteria.DTOs.Proyecto.Response
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