using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using TestTask.AppDb;
using TestTask.Dto;
using TestTask.Models;

namespace TestTask.Controllers
{
    public class CatalogController : Controller
    {
        protected readonly AppDbContext _context;

        public CatalogController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? id)
        {
            var catalogs = _context.Catalogs
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
                    }).ToList()
                }).ToList();

            if (id != null)
            {
                var catalogTitle = _context.Catalogs.Where(c => c.CatalogId == id).Select(c => c.Name).FirstOrDefault();
                ViewBag.CatalogTitle = "Folder - " + catalogTitle;
            }
            else
            {
                ViewBag.CatalogTitle = "Folder - Creating Digital Images";
            }

            return View(catalogs);
        }

        [HttpPost]
        public IActionResult ImportCatalogStructure(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file");
            }

            try
            {
                using (var streamReader = new StreamReader(file.OpenReadStream()))
                {
                    var jsonContent = streamReader.ReadToEnd();
                    var importedCatalogs = JsonConvert.DeserializeObject<List<CatalogDto>>(jsonContent);
                    foreach (var catalogDto in importedCatalogs)
                    {
                        SaveCatalogDto(catalogDto, null);
                    }
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during import: {ex}");

                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        private void SaveCatalogDto(CatalogDto dto, Catalog parentCatalog)
        {
            var catalogEntity = new Catalog
            {
                CatalogId = dto.CatalogId,
                Name = dto.Name,
                ParentCatalog = parentCatalog
            };

            _context.Catalogs.Add(catalogEntity);
            _context.SaveChanges();

            if (dto.SubCatalogs != null)
            {
                foreach (var subCatalogDto in dto.SubCatalogs)
                {
                    SaveCatalogDto(subCatalogDto, catalogEntity);
                }
            }
        }

        [HttpGet]
        public IActionResult ExportCatalogStructure()
        {
            try
            {
                var catalogs = _context.Catalogs.Include(c => c.SubCatalogs).ToList();
                var jsonSettings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                };

                var jsonContent = JsonConvert.SerializeObject(catalogs, Formatting.Indented, jsonSettings);

                var fileName = "exported_catalogs.json";
                return File(Encoding.UTF8.GetBytes(jsonContent), "application/json", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

    }
}
