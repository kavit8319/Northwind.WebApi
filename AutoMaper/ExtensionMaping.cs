using AutoMapper;

namespace Northwind.WebApi.AutoMaper
{
    public static class ExtensionMaping
    {
        public static TDestination Map<TSource1, TSource2, TDestination>(
            this IMapper mapper, TSource1 source1, TSource2 source2)
        {
            var destination = mapper.Map<TSource1, TDestination>(source1);
            return mapper.Map(source2, destination);
        }

        public static void AddMapProfilers(this IServiceCollection service)
        {
            var array = new [] {
             typeof(UserProfiller),
             typeof(SupplierProfiller),
             typeof(CategoryProfiler),
             typeof(ProductProfiler),
             typeof(EmployeeProfiler),
             typeof(TitleProfile),
             typeof(ReportToProfile),
             typeof(CustomerProfiler),
             typeof(ShipperProfile),
             typeof(OrderProfile),
             typeof(EmployeeAllProfile),
             typeof(OrderDetailProfile)
            };
            service.AddAutoMapper(array);
        }
    }
}
