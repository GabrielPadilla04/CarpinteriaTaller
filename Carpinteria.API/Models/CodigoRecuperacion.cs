using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("codigos_recuperacion")]
    public class CodigoRecuperacion
    {
        [Key]
        [Column("codigo_id")]
        public int CodigoId { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        [Required]
        [Column("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [Column("fecha_expiracion")]
        public DateTime FechaExpiracion { get; set; }

        [Column("utilizado")]
        public bool Utilizado { get; set; } = false;
    }
}