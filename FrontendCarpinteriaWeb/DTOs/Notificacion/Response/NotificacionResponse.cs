namespace FrontendCarpinteriaWeb.DTOs.Notificacion.Response
{
    public record NotificacionResponse(
        int NotificacionId,
        int UsuarioId,
        string Titulo,
        string Mensaje,
        bool Leido,
        DateTime FechaHora
    );
}