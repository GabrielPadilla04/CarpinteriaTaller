using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("bitacora_actividades")]
    public class BitacoraActividad
    {
        [Key]
        [Column("log_id")]
        public int LogId { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        [Required]
        [Column("descripcion_accion")]
        public string DescripcionAccion { get; set; } = string.Empty;

        [Required]
        [Column("modulo_afectado")]
        public string ModuloAfectado { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("tipo_accion")]
        public string? TipoAccion { get; set; }

        [Column("registro_afectado_id")]
        public int? RegistroAfectadoId { get; set; }

        [Column("datos_cambio", TypeName = "text")]
        public string? DatosCambio { get; set; }

        [Column("fecha_hora")]
        public DateTime FechaHora { get; set; } = DateTime.Now;
    }
}