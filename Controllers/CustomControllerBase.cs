using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Northwind.WebApi.Model.Data;

namespace Northwind.WebApi.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        protected  readonly NorthwindContext context;
        protected readonly IMapper mapper;

        public CustomControllerBase(NorthwindContext _cont, IMapper _map)
        {
            context = _cont;
            mapper = _map;
        }
    }
}
