using Serilog;                                
using Serilog.Events;
using Serilog.Formatting.Compact;
using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindApp.API.Middleware;
using NorthwindApp.Business.Mapping;
using NorthwindApp.Business.Validation;
using NorthwindApp.Data.Context;
using NorthwindApp.Data.Extensions;
using NorthwindApp.Data.Repositories;
using NorthwindApp.Core.Results;
using NorthwindApp.Business.Services.Concrete;
using NorthwindApp.Business.Services.Abstract;

var builder = WebApplication.CreateBuilder(args);

// Serilog Konfigürasyonu
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)    
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(new CompactJsonFormatter(), "Logs/log-.json", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();                            


// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Memory Cache & ICacheService
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p =>
    {
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod();
    });
});

// Katman Bağımlılıkları
builder.Services.AddNorthwindData(builder.Configuration);
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// FluentValidation
builder.Services.AddFluentValidationAutoValidation(options =>
{
    options.DisableDataAnnotationsValidation = true;
});
builder.Services.AddValidatorsFromAssembly(typeof(ProductCreateDtoValidator).Assembly);

// ModelState hatalarını ApiResponse formatında dön
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value.Errors.Count > 0)
            .SelectMany(x => x.Value.Errors)
            .Select(x => x.ErrorMessage);
        var message = string.Join(", ", errors);
        return new BadRequestObjectResult(ApiResponse<string>.Fail(message));
    };
});

var app = builder.Build();

// Development ortamında Swagger’ı aktif et
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Serilog isteği otomatik loglar
app.UseSerilogRequestLogging();

// Middleware’ler
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
