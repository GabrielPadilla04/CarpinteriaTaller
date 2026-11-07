using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Required]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [Column("apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("roles_id")]
        public int RolesId { get; set; }

        [ForeignKey("RolesId")]
        public Roles? Roles { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Nuevos campos para la recuperación de contraseña por OTP
        [Column("codigo_recuperacion")]
        public string? CodigoRecuperacion { get; set; }

        [Column("expiracion_codigo")]
        public DateTime? ExpiracionCodigo { get; set; }

        // Relaciones correctas con el dominio de la carpintería
        public ICollection<Proyecto> Proyectos { get; set; } = new List<Proyecto>();
        public ICollection<VisitaEntrega> VisitasEntregas { get; set; } = new List<VisitaEntrega>();
    }
}