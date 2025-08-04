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
                options.UseSqlServer(configuration.GetConnectionString("NorthwindConnection")));

<<<<<<< HEAD
            // Generic Repository
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Specific Repositories
=======
            // Register generic repository
            services.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>));
            
            // Register specific repositories
>>>>>>> 46be2e785b2a73d21b3c223b730360640f942087
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
