using Microsoft.EntityFrameworkCore;
using TestTask.Models;

namespace TestTask.AppDb
{
    public class AppDbContext : DbContext
    {
        public DbSet<Catalog> Catalogs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Catalog>()
                .HasMany(subcCatalog => subcCatalog.SubCatalogs)
                .WithOne(parentCatalog => parentCatalog.ParentCatalog)
                .HasForeignKey(prId => prId.ParentCatalogId);

            modelBuilder.Entity<Catalog>().HasData(
                new Catalog { CatalogId = 1, Name = "Creating Digital Images"},
                new Catalog { CatalogId = 2, Name = "Resources", ParentCatalogId = 1 },
                new Catalog { CatalogId = 3, Name = "Evidence", ParentCatalogId = 1 },
                new Catalog { CatalogId = 4, Name = "Grafic Products", ParentCatalogId = 1 },
                new Catalog { CatalogId = 5, Name = "Primary Sources", ParentCatalogId = 2 },
                new Catalog { CatalogId = 6, Name = "Secondary Sources", ParentCatalogId = 2 },
                new Catalog { CatalogId = 7, Name = "Process", ParentCatalogId = 4 },
                new Catalog { CatalogId = 8, Name = "Final Products", ParentCatalogId = 4 }
                );
        }
    }
}
