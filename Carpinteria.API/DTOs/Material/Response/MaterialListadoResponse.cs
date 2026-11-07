namespace TP5_Servicios_API_REST.DTOs.Material.Response
{
    public record MaterialListadoResponse(
        int MaterialId,
        string MarcaProveedor,
        string NombreColorDiseno,
        string? Acabado,
        string? EspesorDisponible,
        string? NombreCategoria
    );
}