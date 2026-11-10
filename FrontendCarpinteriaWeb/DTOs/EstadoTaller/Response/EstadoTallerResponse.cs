namespace FrontendCarpinteriaWeb.DTOs.EstadoTaller
{
	public record EstadoTallerResponse
	{
		public int EstadoTallerId { get; set; }
		public string NombreEstadoTaller { get; set; } = string.Empty;
	}
}