namespace TP5_Servicios_API_REST.DTOs.ProyectoMaterial.Response
{
    public record ProyectoMaterialResponse(
        int ProyectoId,
        int MaterialId,
        string NombreMaterial,
        string MarcaProveedor,
        string Acabado,
        int Cantidad
    );
}