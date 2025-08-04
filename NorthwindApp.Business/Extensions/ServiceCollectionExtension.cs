using Microsoft.Extensions.DependencyInjection;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Business.Services.Concrete;

namespace NorthwindApp.Business.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddNorthwindBusinessServices(this IServiceCollection services)
        {
            // Business Services
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IOrderService, OrderService>();

            // Cache Service
            services.AddSingleton<ICacheService, MemoryCacheService>();

            return services;
        }
    }
} 