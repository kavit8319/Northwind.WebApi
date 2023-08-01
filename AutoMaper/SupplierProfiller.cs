using AutoMapper;
using Northwind.WebApi.Model;
using Northwind.WebApi.Model.Models;

namespace Northwind.WebApi.AutoMaper
{
    public class SupplierProfiller:Profile
    {
        public SupplierProfiller()
        {
            CreateMap<SelectSuppliersPagingResult, SupplierReturn>().ForMember(el=>el.Id,el=>el.MapFrom(el=>el.SupplierID));
            CreateMap<SupplierReturn, Supplier>().ForMember(el=>el.SupplierId,el=>el.MapFrom(el=>el.Id));
            CreateMap<Supplier, SupplierReturn>().ForMember(el=>el.Id,el=>el.MapFrom(el=>el.SupplierId));
        }
    }
}
