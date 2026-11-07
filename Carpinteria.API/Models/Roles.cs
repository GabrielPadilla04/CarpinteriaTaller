using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("roles")]
    public class Roles
    {
        [Key]
        [Column("roles_id")]
        public int RolesId { get; set; }

        [Required]
        [Column("nombre_roles")]
        public string NombreRoles { get; set; } = string.Empty;

        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}