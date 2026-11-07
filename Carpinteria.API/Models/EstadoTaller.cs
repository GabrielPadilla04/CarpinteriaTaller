using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("estados_taller")]
    public class EstadoTaller
    {
        [Key]
        [Column("estado_taller_id")]
        public int EstadoTallerId { get; set; }

        [Required]
        [MaxLength(80)]
        [Column("nombre_estado_taller")]
        public string NombreEstadoTaller { get; set; } = string.Empty;

        // Relación inversa opcional
        public ICollection<Proyecto>? Proyectos { get; set; }
    }
}