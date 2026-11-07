using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("proyectos")]
    public class Proyecto
    {
        [Key]
        [Column("proyecto_id")]
        public int ProyectoId { get; set; }

        [Column("cliente_id")]
        public int ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; } // Diseñador / Carpintero asignado
        public Usuario? Usuario { get; set; }

        [Required]
        [Column("nombre_proyecto")]
        public string NombreProyecto { get; set; } = string.Empty;

        [Column("estado_id")]
        public int EstadoId { get; set; }
        public EstadoProyecto? EstadoProyecto { get; set; }

        [Column("estado_taller_id")]
        public int EstadoTallerId { get; set; } // Reemplazado por ID hacia estados_taller
        [ForeignKey("EstadoTallerId")]
        public EstadoTaller? EstadoTaller { get; set; }

        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public ICollection<Ambiente> Ambientes { get; set; } = new List<Ambiente>();
        public ICollection<HistorialProyecto> HistorialProyectos { get; set; } = new List<HistorialProyecto>();
        public ICollection<VisitaEntrega> VisitasEntregas { get; set; } = new List<VisitaEntrega>();
    }
}