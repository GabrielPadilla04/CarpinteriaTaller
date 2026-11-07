using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("visitas_entregas")]
    public class VisitaEntrega
    {
        [Key]
        [Column("visita_id")]
        public int VisitaId { get; set; }

        [Column("proyecto_id")]
        public int ProyectoId { get; set; }
        public Proyecto? Proyecto { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; } // Operario encargado
        public Usuario? Usuario { get; set; }

        [Required]
        [Column("tipo_evento")]
        public string TipoEvento { get; set; } = string.Empty; // 'Visita de Presupuesto', 'Entrega de Mueble'

        [Column("fecha")]
        public DateOnly Fecha { get; set; }

        [Column("hora_inicio")]
        public TimeOnly HoraInicio { get; set; }

        [Column("hora_fin")]
        public TimeOnly HoraFin { get; set; }
    }
}