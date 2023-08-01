using System.ComponentModel.DataAnnotations;

namespace Northwind.WebApi.Model
{
    public class ShipperReturn:BaseModel
    {
        public string CompanyName { get; set; }

        public string Phone { get; set; }
    }
}
