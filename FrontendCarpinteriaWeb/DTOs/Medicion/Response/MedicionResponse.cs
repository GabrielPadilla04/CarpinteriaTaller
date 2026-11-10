namespace FrontendCarpinteriaWeb.DTOs.Medicion.Response
{
    public record MedicionResponse(
        int MedicionId,
        int AmbienteId,
        string EtiquetaModulo,
        int AltoMm,
        int AnchoMm,
        int ProfundidadMm,
        string? UbicacionModulo,
        string? NotesModulo
    );
}