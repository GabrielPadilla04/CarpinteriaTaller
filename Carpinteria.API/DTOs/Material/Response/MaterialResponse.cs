using System.Text.Json.Serialization;

namespace Carpinteria.API.DTOs.Material.Response
{
    public record MaterialResponse(
        [property: JsonPropertyName("materialId")] int MaterialId,
        [property: JsonPropertyName("categoriaId")] int CategoriaId,
        [property: JsonPropertyName("nombreCategoria")] string? NombreCategoria,
        [property: JsonPropertyName("marcaProveedor")] string MarcaProveedor,
        [property: JsonPropertyName("nombreColorDiseno")] string NombreColorDiseno,
        [property: JsonPropertyName("acabado")] string? Acabado,
        [property: JsonPropertyName("espesorDisponible")] string? EspesorDisponible,
        [property: JsonPropertyName("imagenUrl")] string? ImagenUrl
    );
}