using AutoMapper;
using Northwind.WebApi.Model;
using Northwind.WebApi.Model.Models;

namespace Northwind.WebApi.AutoMaper
{
    public class TitleProfile:Profile
    {
        public TitleProfile()
        {
            CreateMap<GetTitlesResult, TitleReturn>();
        }
    }
}
