using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("compras")]
    public class Compra
    {
        [Key]
        [Column("compra_id")]
        public int CompraId { get; set; }

        [Required]
        [Column("proveedor_id")]
        public int ProveedorId { get; set; }

        [ForeignKey("ProveedorId")]
        public Proveedor? Proveedor { get; set; }

        [Required]
        [Column("material_id")]
        public int MaterialId { get; set; }

        [ForeignKey("MaterialId")]
        public Material? Material { get; set; }

        [Required]
        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Required]
        [Column("precio_unitario", TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [Column("fecha_compra")]
        public DateTime FechaCompra { get; set; } = DateTime.Now;

        [MaxLength(255)]
        [Column("observaciones")]
        public string? Observaciones { get; set; }
    }
}