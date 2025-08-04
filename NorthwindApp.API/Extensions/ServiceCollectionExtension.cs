using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using NorthwindApp.API.Middleware;
using NorthwindApp.Business.Extensions;
using NorthwindApp.Business.Mapping;
using NorthwindApp.Business.Validation;
using NorthwindApp.Core.Results;

namespace NorthwindApp.API.Extensions
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddNorthwindServices(this IServiceCollection services)
        {
            // Business Services
            services.AddNorthwindBusinessServices();
            return services;
        }

        public static IServiceCollection AddNorthwindAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(MappingProfile).Assembly);
            return services;
        }

        public static IServiceCollection AddNorthwindValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation(options =>
            {
                options.DisableDataAnnotationsValidation = true;
            });
            services.AddValidatorsFromAssembly(typeof(ProductCreateDtoValidator).Assembly);
            return services;
        }

        public static IServiceCollection AddNorthwindCaching(this IServiceCollection services)
        {
            services.AddMemoryCache();
            return services;
        }

        public static IServiceCollection AddNorthwindCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", p =>
                {
                    p.AllowAnyOrigin()
                     .AllowAnyHeader()
                     .AllowAnyMethod();
                });
            });
            return services;
        }

        public static IServiceCollection AddNorthwindApiBehavior(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    // ModelState içindeki tüm hata mesajlarını topla:
                    var errors = context.ModelState
                        .Where(ms => ms.Value?.Errors.Count > 0)
                        .SelectMany(ms => ms.Value?.Errors ?? Enumerable.Empty<Microsoft.AspNetCore.Mvc.ModelBinding.ModelError>())
                        .Select(e => e?.ErrorMessage ?? "Bilinmeyen hata")
                        .ToList();

                    // ApiResponse.BadRequest factory'si, hem Success=false, hem StatusCode=400, hem de Error listesi içerir:
                    var apiResponse = ApiResponse<string>.BadRequest(
                        errors,
                        message: "Geçersiz istek. Lütfen gönderdiğiniz verileri kontrol ediniz."
                    );

                    // BadRequestObjectResult içinde bu ApiResponse'u dön:
                    var result = new BadRequestObjectResult(apiResponse)
                    {
                        // MVC default 400'ü zaten ayarlar, ancak StatusCode alanını explicit eşitlemek istersen:
                        StatusCode = apiResponse.StatusCode
                    };

                    return result;
                };
            });
            return services;
        }

        public static IServiceCollection AddNorthwindMiddleware(this IServiceCollection services)
        {
            // Middleware'ler DI container'a register edilmez, UseMiddleware ile kullanılır
            return services;
        }
    }
} 