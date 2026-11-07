using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("notificaciones")]
    public class Notificacion
    {
        [Key]
        [Column("notificacion_id")]
        public int NotificacionId { get; set; }

        [Required]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Required]
        [Column("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [Column("mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        [Column("leido")]
        public bool Leido { get; set; } = false;

        [Column("fecha_hora")]
        public DateTime FechaHora { get; set; } = DateTime.Now;

        public Usuario? Usuario { get; set; }
    }
}