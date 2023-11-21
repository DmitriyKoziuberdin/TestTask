using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TestTask.AppDb;
using TestTask.Dto;
using TestTask.Interfaces;
using TestTask.Models;

namespace TestTask.Services
{
    public class CatalogService : ICatalogService
    {
        private readonly AppDbContext _context;

        public CatalogService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<CatalogDto>> GetCatalogs(int? id)
        {
            var catalogs = await _context.Catalogs
                .Include(c => c.SubCatalogs)
                .Where(c => id == null ? c.ParentCatalogId == null : c.ParentCatalogId == id)
                .Select(c => new CatalogDto
                {
                    CatalogId = c.CatalogId,
                    Name = c.Name,
                    SubCatalogs = c.SubCatalogs.Select(sub => new CatalogDto
                    {
                        CatalogId = sub.CatalogId,
                        Name = sub.Name,
                        ParentCatalogId = sub.ParentCatalogId,
                        SubCatalogs = null
                    }).ToList()
                }).ToListAsync();

            return catalogs;
        }

        public async Task<string> GetCatalogName(int catalogId)
        {
            return await _context.Catalogs
                .Where(c => c.CatalogId == catalogId)
                .Select(c => c.Name)
                .FirstOrDefaultAsync();
        }

        public async Task ImportCatalogStructure(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new Exception("Invalid file");
            }

            try
            {
                using (var streamReader = new StreamReader(file.OpenReadStream()))
                {
                    var jsonContent = await streamReader.ReadToEndAsync();
                    var importedCatalogs = JsonConvert.DeserializeObject<List<CatalogDto>>(jsonContent);
                    foreach (var catalogDto in importedCatalogs)
                    {
                        await SaveCatalogDto(catalogDto, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during import: {ex}");
                throw;
            }
        }

        private async Task SaveCatalogDto(CatalogDto dto, Catalog parentCatalog)
        {
            var catalogEntity = new Catalog
            {
                CatalogId = dto.CatalogId,
                Name = dto.Name,
                ParentCatalog = parentCatalog
            };

            _context.Catalogs.Add(catalogEntity);
            await _context.SaveChangesAsync();

            if (dto.SubCatalogs != null)
            {
                foreach (var subCatalogDto in dto.SubCatalogs)
                {
                    await SaveCatalogDto(subCatalogDto, catalogEntity);
                }
            }
        }

        public async Task ExportCatalogStructure()
        {
            try
            {
                var catalogs = await _context.Catalogs.Include(c => c.SubCatalogs).ToListAsync();

                var exportDtos = catalogs.Select(c => new CatalogDto
                {
                    CatalogId = c.CatalogId,
                    Name = c.Name,
                    ParentCatalogId = c.ParentCatalogId,
                    SubCatalogs = c.SubCatalogs.Select(sub => new CatalogDto
                    {
                        CatalogId = sub.CatalogId,
                        Name = sub.Name,
                        ParentCatalogId = sub.ParentCatalogId,
                        SubCatalogs = null
                    }).ToList()
                }).ToList();

                var jsonSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                var jsonContent = JsonConvert.SerializeObject(exportDtos, Formatting.Indented, jsonSettings);

                var fileName = "exported_catalogs.json";
                await System.IO.File.WriteAllTextAsync(fileName, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during export: {ex}");
                throw;
            }
        }
    }
}
