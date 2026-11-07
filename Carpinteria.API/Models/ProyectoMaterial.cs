using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
    [Table("proyectos_materiales")]
    public class ProyectoMaterial
    {
        [Column("proyecto_id")]
        public int ProyectoId { get; set; }

        [ForeignKey(nameof(ProyectoId))]
        public Proyecto? Proyecto { get; set; }

        [Column("material_id")]
        public int MaterialId { get; set; }

        [ForeignKey(nameof(MaterialId))]
        public Material? Material { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }
    }
}