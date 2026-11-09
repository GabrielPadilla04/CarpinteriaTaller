namespace FrontendCarpinteria.DTOs.ProyectoMaterial.Response
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