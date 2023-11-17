using System.Text.Json.Serialization;

namespace TestTask.Models
{
    public class Catalog
    {
        public int CatalogId { get; set; }
        public string? Name { get; set; }
        public int? ParentCatalogId { get; set; }
        [JsonIgnore]
        public Catalog? ParentCatalog { get; set; }
        [JsonIgnore]
        public List<Catalog>? SubCatalogs { get; set; }
    }
}
