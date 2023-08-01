namespace Northwind.WebApi.Model
{
    public class CustomerReturn:BaseModel
    {
        public string CompanyName { get; set; }
        public string CustomerTitle { get; set; }
        public int? CustomerTitleId { get; set; }
        public string ContactName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public bool? IsVip { get; set; }
    }
}
