namespace FrontendCarpinteriaWeb.DTOs.Bitacora.Response
{
    public record BitacoraResponse(
        int LogId,
        int UsuarioId,
        string DescripcionAccion,
        string ModuloAfectado,
        DateTime FechaHora
    );
}