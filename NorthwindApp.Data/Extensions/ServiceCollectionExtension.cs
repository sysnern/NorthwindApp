using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthwindApp.Data.Context;

namespace NorthwindApp.Data.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddNorthwindData(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<NorthwindContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("NorthwindConnection"));
            });

            // Register your data services here
            // Example: services.AddScoped<IProductService, ProductService>();

            return services;
        }
    }
}
