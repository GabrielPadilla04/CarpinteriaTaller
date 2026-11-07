using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("categorias_material")]
    public class CategoriaMaterial
    {
        [Key]
        [Column("categoria_id")]
        public int CategoriaId { get; set; }

        [Required]
        [Column("nombre_categoria")]
        public string NombreCategoria { get; set; } = string.Empty;
    }
}