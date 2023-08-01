using AutoMapper;
using Northwind.WebApi.Model;
using Northwind.WebApi.Model.Models;

namespace Northwind.WebApi.AutoMaper
{
    public class OrderDetailProfile:Profile
    {
        public OrderDetailProfile()
        {
            CreateMap<OrderDetail, OrderDetailReturn>().ForMember(el => el.Id, el => el.MapFrom(el => el.OrderId))
                .ReverseMap();
            CreateMap<SelectOrdersDetailResult, OrderDetailReturn>()
                .ForMember(el => el.Id, el => el.MapFrom(el => el.OrderID)).ReverseMap();
            
        }
    }
}
