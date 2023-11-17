using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TestTask.AppDb;
using TestTask.Dto;

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

    }
}
