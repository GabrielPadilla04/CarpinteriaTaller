using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("clientes")]
    public class Cliente
    {
        [Key]
        [Column("cliente_id")]
        public int ClienteId { get; set; }

        [Required]
        [Column("nombre_apellido")]
        public string NombreApellido { get; set; } = string.Empty;

        [Required]
        [Column("telefono")]
        public string Telefono { get; set; } = string.Empty;

        [Required]
        [Column("direccion_obra")]
        public string DireccionObra { get; set; } = string.Empty;
    }
}