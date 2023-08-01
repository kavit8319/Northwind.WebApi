namespace Northwind.WebApi.Model
{
    public class OrderDetailReturn:BaseModel
    {
        public string ProductName { get; set; }
        public int ProductID { get; set; }
        public short? UnitsInStock { get; set; }
        public short? Quantity { get; set; }
        public decimal? UnitPrice { get; set; }
        public float? Discount { get; set; }
        public double? TotalSumm { get;set; }
    }
}
