using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("proveedores_materiales")]
    public class ProveedorMaterial
    {
        [Column("id_proveedor")]
        public int IdProveedor { get; set; }

        [ForeignKey("IdProveedor")]
        public Proveedor? Proveedor { get; set; }

        [Column("id_material")]
        public int IdMaterial { get; set; }

        [ForeignKey("IdMaterial")]
        public Material? Material { get; set; }

        [Column("precio_compra", TypeName = "decimal(18,2)")]
        public decimal PrecioCompra { get; set; }

        [Column("fecha_actualizacion", TypeName = "date")]
        public DateTime FechaActualizacion { get; set; } = DateTime.Now;

        [MaxLength(100)]
        [Column("codigo_producto_proveedor")]
        public string? CodigoProductoProveedor { get; set; }

        [MaxLength(50)]
        [Column("unidad_compra")]
        public string? UnidadCompra { get; set; }
    }
}