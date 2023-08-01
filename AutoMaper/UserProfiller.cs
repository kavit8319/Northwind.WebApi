using AutoMapper;
using Northwind.WebApi.Model.Models;
using Northwind.WebApi.ModelsExt.ModelAuthentic;

namespace Northwind.WebApi.AutoMaper
{
    public class UserProfiller: Profile
    {
        public UserProfiller()
        {
            CreateMap<AdmUser, UserReturn>().ReverseMap();
            CreateMap<AdmUser, LogoneResult>();
            CreateMap<UserWithToken, UserReturn>()
                .ForMember(el=>el.RoleId,opt=>opt.MapFrom(elm=>elm.Role.RoleId))
                .ForMember(elmemnt=>elmemnt.RoleName,t=>t.MapFrom(elm=>elm.Role.Name));
            CreateMap<LogoneResult, UserReturn>().ReverseMap();
            CreateMap<UserByIdResult, UserReturn>();
            CreateMap<LogoneResult, UserByIdResult>();
            CreateMap<UserReturn, AdmUser>();
            CreateMap<UserByEmailResult, AdmUser>();
            CreateMap<SelectUserPagingResult, UserReturn>();
        }
    }
}
