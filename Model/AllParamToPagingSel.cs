namespace Northwind.WebApi.Model
{
    public class AllParamToPagingSel
    {
        public string? SortCollName { get; set; }
        public string? AscDesc { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? FilterType { get; set; }
    }
}
