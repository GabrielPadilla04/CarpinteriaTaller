namespace TP5_Servicios_API_REST.DTOs.Bitacora.Response
{
    public record BitacoraResponse(
        int LogId,
        int UsuarioId,
        string DescripcionAccion,
        string ModuloAfectado,
        DateTime FechaHora
    );
}