namespace Northwind.WebApi.Model
{
    public class CategoryView:BaseModel
    {
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public byte[]? Picture { get; set; }
        //public StreamContent StreamContent { get; set; }
    }
}
