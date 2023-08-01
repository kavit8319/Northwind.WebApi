namespace Northwind.WebApi.Model
{
    public class OrderReturn:BaseModel
    {
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public int ShipVia { get; set; }
        public string ShipCompanyName { get; set; }
        public string ShipPhone { get; set; }
        public string? CompanyName { get; set; }
        public string? ContactName { get; set; }
        public string? EmplFullName { get; set; }
        public decimal? Freight { get; set; }
        public string? ShipName { get; set; }
        public string? ShipAddress { get; set; }
        public string Country { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public int? CountPositions { get; set; }
        public int? SummQuantity { get; set; }
        public double? TotalSumm { get; set; }
        public DateTime? OrderDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public DateTime? ShippedDate { get; set; }
    }
}
