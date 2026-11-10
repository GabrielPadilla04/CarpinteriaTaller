using System.Text.Json.Serialization;

namespace FrontendCarpinteria.DTOs.Material.Response
{
    public record MaterialListadoResponse(
        [property: JsonPropertyName("materialId")] int MaterialId,
        [property: JsonPropertyName("categoriaId")] int CategoriaId,
        [property: JsonPropertyName("marcaProveedor")] string MarcaProveedor,
        [property: JsonPropertyName("nombreColorDiseno")] string NombreColorDiseno,
        [property: JsonPropertyName("acabado")] string? Acabado,
        [property: JsonPropertyName("espesorDisponible")] string? EspesorDisponible,
        [property: JsonPropertyName("nombreCategoria")] string? NombreCategoria,
        [property: JsonPropertyName("imagenUrl")] string? ImagenUrl

    );
}