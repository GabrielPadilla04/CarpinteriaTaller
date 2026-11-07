using System.ComponentModel.DataAnnotations;

namespace TP5_Servicios_API_REST.DTOs.Medicion.Request
{
    public class CrearMedicionRequest
    {
        [Required(ErrorMessage = "El ID del ambiente es obligatorio.")]
        public int AmbienteId { get; set; }

        [Required(ErrorMessage = "La etiqueta del módulo es obligatoria.")]
        public string EtiquetaModulo { get; set; } = string.Empty;

        public int AltoMm { get; set; }
        public int AnchoMm { get; set; }
        public int ProfundidadMm { get; set; }
        public string? UbicacionModulo { get; set; }
        public string? NotesModulo { get; set; }
    }
}