using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("proveedores")] // O el nombre exacto de tu tabla en MariaDB
    public class Proveedor
    {
        [Key]
        [Column("id_proveedor")]
        public int IdProveedor { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("razon_social")]
        public string RazonSocial { get; set; } = string.Empty;

        [Required]
        [MaxLength(13)]
        [Column("cuit")]
        public string Cuit { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("nombre_contacto")]
        public string? NombreContacto { get; set; }

        [MaxLength(20)]
        [Column("telefono")]
        public string? Telefono { get; set; }

        [MaxLength(100)]
        [Column("email")]
        public string? Email { get; set; }

        [MaxLength(150)]
        [Column("direccion")]
        public string? Direccion { get; set; }

        [MaxLength(80)]
        [Column("localidad")]
        public string? Localidad { get; set; }

        [MaxLength(80)]
        [Column("provincia")]
        public string? Provincia { get; set; }

        [MaxLength(50)]
        [Column("tipo_proveedor")]
        public string? TipoProveedor { get; set; }

        [Column("fecha_alta", TypeName = "date")]
        public DateTime FechaAlta { get; set; } = DateTime.Now;

        [Column("estado")]
        public bool Estado { get; set; } = true;
    }
}