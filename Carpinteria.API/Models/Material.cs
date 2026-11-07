using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("materiales")]
    public class Material
    {
        [Key]
        [Column("material_id")]
        public int MaterialId { get; set; }

        [Column("categoria_id")]
        public int CategoriaId { get; set; }

        [ForeignKey("CategoriaId")]
        public CategoriaMaterial? CategoriaMaterial { get; set; }

        [Required]
        [Column("marca_proveedor")]
        public string MarcaProveedor { get; set; } = string.Empty;

        [Required]
        [Column("nombre_color_diseno")]
        public string NombreColorDiseno { get; set; } = string.Empty;

        [Column("acabado")]
        public string? Acabado { get; set; }

        [Column("espesor_disponible")]
        public string? EspesorDisponible { get; set; }
    }
}