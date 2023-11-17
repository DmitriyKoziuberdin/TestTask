namespace TestTask.Dto
{
    public class CatalogDto
    {
        public int CatalogId { get; set; }
        public string? Name { get; set; }
        public List<CatalogDto>? SubCatalogs { get; set; }
    }
}
