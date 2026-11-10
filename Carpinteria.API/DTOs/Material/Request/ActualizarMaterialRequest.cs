using System.ComponentModel.DataAnnotations;

namespace TP5_Servicios_API_REST.DTOs.Material.Request
{
    public class ActualizarMaterialRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Debe seleccionar una categoría válida.")]
        public int CategoriaId { get; set; }

        [Required(ErrorMessage = "La marca o proveedor es obligatorio.")]
        public string MarcaProveedor { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre/color/diseño es obligatorio.")]
        public string NombreColorDiseno { get; set; } = string.Empty;

        public string? Acabado { get; set; }
        public string? EspesorDisponible { get; set; }
        public string? ImagenUrl { get; set; }
    }
}