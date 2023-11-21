using Microsoft.AspNetCore.Mvc;
using TestTask.Interfaces;

namespace TestTask.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ICatalogService _catalogService;

        public CatalogController(ICatalogService catalogService)
        {
            _catalogService = catalogService;
        }

        public async Task<IActionResult> Index(int? id)
        {
            var catalogs = await _catalogService.GetCatalogs(id);

            if (id != null)
            {
                var catalogTitle = await _catalogService.GetCatalogName(id.Value);
                ViewBag.CatalogTitle = "Folder - " + catalogTitle;
            }
            else
            {
                ViewBag.CatalogTitle = "Folder - Creating Digital Images";
            }

            return View(catalogs);
        }

        [HttpPost]
        public async Task<IActionResult> ImportCatalogStructure(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file");
            }

            try
            {
                await _catalogService.ImportCatalogStructure(file);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during import: {ex}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ExportCatalogStructure()
        {
            try
            {
                await _catalogService.ExportCatalogStructure();

                var fileName = "exported_catalogs.json";
                var fileBytes = await System.IO.File.ReadAllBytesAsync(fileName);

                return File(fileBytes, "application/json", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during export: {ex}");
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
    }
}
