using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("mediciones")]
    public class Medicion
    {
        [Key]
        [Column("medicion_id")]
        public int MedicionId { get; set; }

        [Column("ambiente_id")]
        public int AmbienteId { get; set; }
        public Ambiente? Ambiente { get; set; }

        [Required]
        [Column("etiqueta_modulo")]
        public string EtiquetaModulo { get; set; } = string.Empty; // 'Alacena Superior', 'Bajo Mesada A'

        [Column("alto_mm")]
        public int AltoMm { get; set; }

        [Column("ancho_mm")]
        public int AnchoMm { get; set; }

        [Column("profundidad_mm")]
        public int ProfundidadMm { get; set; }

        [Column("ubicacion_modulo")]
        public string? UbicacionModulo { get; set; }

        [Column("notas_modulo")]
        public string? NotesModulo { get; set; }
    }
}