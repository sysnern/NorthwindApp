using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindApp.API.Middleware;
using NorthwindApp.Business.Mapping;
using NorthwindApp.Business.Services.Abstract;
using NorthwindApp.Business.Services.Concrete;
using NorthwindApp.Business.Validation;
using NorthwindApp.Core.Results;
using NorthwindApp.Data.Context;
using NorthwindApp.Data.Extensions;
using NorthwindApp.Data.Repositories;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;


var builder = WebApplication.CreateBuilder(args);

//logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration!)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", Serilog.Events.LogEventLevel.Debug)
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {SourceContext}{NewLine}{Message:lj}{NewLine}{Exception}")
    .WriteTo.Graylog(new GraylogSinkOptions
    {
        HostnameOrAddress = "localhost",
        Port = 12201,
        TransportType = TransportType.Udp
    })
    .CreateLogger();

builder.Host.UseSerilog();                            

//service registrations(DI)
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ICacheService, MemoryCacheService>();

 
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p =>
    {
        p.AllowAnyOrigin()
         .AllowAnyHeader()
         .AllowAnyMethod();
    });
});

builder.Services.AddNorthwindData(builder.Configuration);

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IOrderService, OrderService>();


builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

builder.Services.AddFluentValidationAutoValidation(options =>
{
    options.DisableDataAnnotationsValidation = true;
});
builder.Services.AddValidatorsFromAssembly(typeof(ProductCreateDtoValidator).Assembly);

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        // ModelState içindeki tüm hata mesajlarını topla:
        var errors = context.ModelState
            .Where(ms => ms.Value.Errors.Count > 0)
            .SelectMany(ms => ms.Value.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        // ApiResponse.BadRequest factory’si, hem Success=false, hem StatusCode=400, hem de Error listesi içerir:
        var apiResponse = ApiResponse<string>.BadRequest(
            errors,
            message: "Geçersiz istek. Lütfen gönderdiğiniz verileri kontrol ediniz."
        );

        // BadRequestObjectResult içinde bu ApiResponse’u dön:
        var result = new BadRequestObjectResult(apiResponse)
        {
            // MVC default 400’ü zaten ayarlar, ancak StatusCode alanını explicit eşitlemek istersen:
            StatusCode = apiResponse.StatusCode
        };

        return result;
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app!.UseSerilogRequestLogging();
app!.UseMiddleware<GlobalExceptionMiddleware>();
app!.UseMiddleware<ValidationExceptionMiddleware>();


app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
