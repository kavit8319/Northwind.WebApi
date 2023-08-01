using AutoMapper;
using Northwind.WebApi.Model;
using Northwind.WebApi.Model.Models;

namespace Northwind.WebApi.AutoMaper
{
    public class ShipperProfile:Profile
    {
        public ShipperProfile()
        {
            CreateMap<Shipper, ShipperReturn>().ForMember(el => el.Id, el => el.MapFrom(el => el.ShipperId)).ReverseMap();
        }
    }
}
