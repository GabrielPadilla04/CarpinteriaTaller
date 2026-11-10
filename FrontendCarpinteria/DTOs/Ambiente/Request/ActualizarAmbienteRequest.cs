using System.ComponentModel.DataAnnotations;

namespace FrontendCarpinteria.DTOs.Ambiente.Request
{
	public class ActualizarAmbienteRequest
	{
		[Required(ErrorMessage = "El ID del proyecto es obligatorio.")]
		public int ProyectoId { get; set; }

		[Required(ErrorMessage = "El nombre del ambiente es obligatorio.")]
		public string NombreAmbiente { get; set; } = string.Empty;
	}
}