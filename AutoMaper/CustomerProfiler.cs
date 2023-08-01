using AutoMapper;
using Northwind.WebApi.Model;
using Northwind.WebApi.Model.Models;

namespace Northwind.WebApi.AutoMaper
{
    public class CustomerProfiler:Profile
    {
        public CustomerProfiler()
        {
            CreateMap<SelectCustomerPagingResult, CustomerReturn>()
                .ForMember(el => el.Id, el => el.MapFrom(el => el.CustomerId));
            CreateMap<CustomerReturn, Customer>().ForMember(el => el.CustomerId, el => el.MapFrom(el => el.Id)).ReverseMap();
        }
    }
}
