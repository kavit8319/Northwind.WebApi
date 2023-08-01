using AutoMapper;
using Northwind.WebApi.Model;
using Northwind.WebApi.Model.Models;

namespace Northwind.WebApi.AutoMaper
{
    public class EmployeeProfiler: Profile
    {
        public EmployeeProfiler()
        {
            CreateMap<SelectEmployeesPagingResult,EmployeeReturn>().ForMember(el => el.Id, el => el.MapFrom(el => el.EmployeeId)).
                ForMember(el=>el.IsParent,el=>el.PreCondition(el=>el.IsParent==1));
            CreateMap<EmployeeReturn,Employee>().ForMember(el => el.EmployeeId, el => el.MapFrom(el => el.Id));
            CreateMap<Employee,EmployeeReturn>();
        }
    }
}
