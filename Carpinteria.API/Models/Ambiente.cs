using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("ambientes")]
    public class Ambiente
    {
        [Key]
        [Column("ambiente_id")]
        public int AmbienteId { get; set; }

        [Column("proyecto_id")]
        public int ProyectoId { get; set; }
        public Proyecto? Proyecto { get; set; }

        [Required]
        [Column("nombre_ambiente")]
        public string NombreAmbiente { get; set; } = string.Empty; // 'Cocina', 'Vestidor', 'Baño'
    }
}