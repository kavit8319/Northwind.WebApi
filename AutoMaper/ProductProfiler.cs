using AutoMapper;
using Northwind.WebApi.Model;
using Northwind.WebApi.Model.Models;

namespace Northwind.WebApi.AutoMaper
{
    public class ProductProfiler:Profile
    {
        public ProductProfiler()
        {
            CreateMap<SelectProductPagingResult, ProductReturn>().ForMember(el=>el.Id,el=>el.MapFrom(el=>el.ProductID));
            CreateMap<ProductReturn, Product>().ForMember(el=>el.ProductId,el=>el.MapFrom(el=>el.Id));
            CreateMap<Product, ProductReturn>().ForMember(el=>el.Id,el=>el.MapFrom(el=>el.ProductId));
        }
    }
}
