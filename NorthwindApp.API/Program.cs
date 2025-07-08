using AutoMapper;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindApp.API.Middleware;
using NorthwindApp.Business.Mapping;
using NorthwindApp.Business.Services;
using NorthwindApp.Business.Validation;
using NorthwindApp.Data.Context;
using NorthwindApp.Data.Extensions;
using NorthwindApp.Data.Repositories;
using NorthwindApp.Core.Results;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (Swagger testleri için)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
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


// AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

// FluentValidation
builder.Services.AddFluentValidationAutoValidation(options =>
{
    options.DisableDataAnnotationsValidation = true;
});
builder.Services.AddValidatorsFromAssembly(typeof(ProductCreateDtoValidator).Assembly);

// ✨ ModelState Validation -> ApiResponse olarak dönmesi için:
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(x => x.Value!.Errors.Count > 0)
            .SelectMany(x => x.Value!.Errors)
            .Select(x => x.ErrorMessage);

        var message = string.Join(", ", errors);
        var apiResponse = ApiResponse<string>.Fail(message);

        return new BadRequestObjectResult(apiResponse);
    };
});

var app = builder.Build();

// Development ortamı için Swagger aktif et
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Hata yönetimi için Middleware (Exception yakalama)
app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
