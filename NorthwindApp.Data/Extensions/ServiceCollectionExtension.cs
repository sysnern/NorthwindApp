using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthwindApp.Data.Context;
using NorthwindApp.Data.Repositories.Abstract;
using NorthwindApp.Data.Repositories.Concrete;

namespace NorthwindApp.Data.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddNorthwindData(this IServiceCollection services, IConfiguration configuration)
        {
            // Database Context
            services.AddDbContext<NorthwindContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register specific repositories
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            return services;
        }
    }
}
