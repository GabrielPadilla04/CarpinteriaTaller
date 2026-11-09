namespace FrontendCarpinteria.DTOs.Material.Response
{
    public record MaterialResponse(
        int MaterialId,
        int CategoriaId,
        string? NombreCategoria,
        string MarcaProveedor,
        string NombreColorDiseno,
        string? Acabado,
        string? EspesorDisponible
    );
}