using Newtonsoft.Json;

namespace TestTask.Dto
{
    public class CatalogDto
    {
        [JsonProperty("CatalogId")]
        public int CatalogId { get; set; }

        [JsonProperty("Name")]
        public string? Name { get; set; }

        [JsonProperty("ParentCatalogId")]
        public int? ParentCatalogId { get; set; }

        [JsonProperty("SubCatalogs")]
        public List<CatalogDto>? SubCatalogs { get; set; }
    }
}
