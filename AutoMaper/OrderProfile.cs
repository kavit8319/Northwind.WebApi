using System.Globalization;
using AutoMapper;
using Northwind.WebApi.Model;
using Northwind.WebApi.Model.Models;

namespace Northwind.WebApi.AutoMaper
{
    public class OrderProfile:Profile
    {
        public OrderProfile()
        {
            CreateMap<SelectOrdersPagingResult, OrderReturn>();
            CreateMap<OrderReturn, Order>().ForMember(el=>el.ShipCountry,el=>el.MapFrom(el=>el.Country))
                .ForMember(el=>el.ShipCity,el=>el.MapFrom(el=>el.City))
                .ForMember(el=>el.ShipPostalCode,el=>el.MapFrom(el=>el.PostalCode))
                .ForMember(el=>el.OrderId,el=>el.MapFrom(el=>el.Id)).ReverseMap();
        }
    }

}
