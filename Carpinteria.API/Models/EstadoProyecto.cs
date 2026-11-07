using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TP5_Servicios_API_REST.Models
{
	[Table("estados_proyecto")]
	public class EstadoProyecto
	{
		[Key]
		[Column("estado_id")]
		public int EstadoId { get; set; }

		[Required]
		[Column("nombre_estado")]
		public string NombreEstado { get; set; } = string.Empty;

		[Column("porcentaje_progreso")]
		public int PorcentajeProgreso { get; set; }
	}
}