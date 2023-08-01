using AutoMapper;
using Northwind.WebApi.Model;
using Northwind.WebApi.Model.Models;

namespace Northwind.WebApi.AutoMaper
{
    public class CategoryProfiler: Profile
    {
        public CategoryProfiler()
        {
            CreateMap<SelectCategoryPagingResult, CategoryView>()
                .ForMember(el => el.Id, el => el.MapFrom(el => el.CategoryID))
                .ForMember(el=>el.Description,el=>el.MapFrom(el=>el.catDescription));
            CreateMap<Category, CategoryView>().ForMember(el=>el.Id,el=>el.MapFrom(el=>el.CategoryId)).ReverseMap();
        }
    }
}
