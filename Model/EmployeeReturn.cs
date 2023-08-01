namespace Northwind.WebApi.Model
{
    public class EmployeeReturn: BaseModel
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string? TitleOfCourtesy { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime? HireDate { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public byte[]? Photo { get; set; }
        public string? Notes { get; set; }
        public int TitleId { get; set; }
        public string TitleName { get; set; }
        public int? ReportsTo { get; set; }
        public int IsParent { get; set; }
        public int Lavel { get; set; }
    }
}
