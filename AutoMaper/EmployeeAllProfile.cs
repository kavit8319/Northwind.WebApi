using AutoMapper;
using Northwind.WebApi.Model;
using Northwind.WebApi.Model.Models;

namespace Northwind.WebApi.AutoMaper
{
    public class EmployeeAllProfile:Profile
    {
        public EmployeeAllProfile()
        {
            CreateMap<SelectEmployeesAllResult, EmployeeAllReturn>();
        }
    }
}
