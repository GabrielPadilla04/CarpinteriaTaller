using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("historial_proyectos")]
    public class HistorialProyecto
    {
        [Key]
        [Column("historial_id")]
        public int HistorialId { get; set; }

        [Column("proyecto_id")]
        public int ProyectoId { get; set; }
        public Proyecto? Proyecto { get; set; }

        [Column("estado_id")]
        public int EstadoId { get; set; }
        public EstadoProyecto? EstadoProyecto { get; set; }

        [Column("fecha_cambio")]
        public DateTime FechaCambio { get; set; } = DateTime.Now;
    }
}