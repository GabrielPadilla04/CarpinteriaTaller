using System.ComponentModel.DataAnnotations;

namespace TP5_Servicios_API_REST.DTOs.Ambiente.Request
{
	public class CrearAmbienteRequest
	{
		[Required(ErrorMessage = "El ID del proyecto es obligatorio.")]
		public int ProyectoId { get; set; }

		[Required(ErrorMessage = "El nombre del ambiente es obligatorio.")]
		public string NombreAmbiente { get; set; } = string.Empty;
	}
}