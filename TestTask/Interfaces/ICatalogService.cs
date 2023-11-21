using TestTask.Dto;

namespace TestTask.Interfaces
{
    public interface ICatalogService
    {
        Task<List<CatalogDto>> GetCatalogs(int? id);
        Task<string> GetCatalogName(int catalogId);
        Task ImportCatalogStructure(IFormFile file);
        Task ExportCatalogStructure();
    }
}
