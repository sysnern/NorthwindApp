using NorthwindApp.API.Extensions;
using NorthwindApp.API.Middleware;
using NorthwindApp.Data.Extensions;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Sinks.Graylog;
using Serilog.Sinks.Graylog.Core.Transport;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Azure App Service için port ayarı
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

//logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration!)
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate:
        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();                            

//service registrations(DI)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Enhanced Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "NorthwindApp API",
        Version = "v1.0.0",
        Description = "Modern .NET 8.0 API for Northwind database management with advanced features including caching, filtering, and soft delete functionality.",
        Contact = new OpenApiContact
        {
            Name = "NorthwindApp Team",
            Email = "support@northwindapp.com",
            Url = new Uri("https://github.com/Sysnern/NorthwindApp")
        },
        License = new OpenApiLicense
        {
            Name = "MIT License",
            Url = new Uri("https://opensource.org/licenses/MIT")
        }
    });

    // XML Documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // Add JWT Authentication (for future use)
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

    // Custom operation filters
    c.OperationFilter<SwaggerDefaultValues>();
    
    // Group endpoints by controller
    c.TagActionsBy(api =>
    {
        if (api.GroupName != null)
        {
            return new[] { api.GroupName.ToString() };
        }

        var controllerActionDescriptor = api.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
        if (controllerActionDescriptor != null)
        {
            return new[] { controllerActionDescriptor.ControllerName };
        }

        throw new InvalidOperationException("Unable to determine tag for endpoint.");
    });

    c.DocInclusionPredicate((name, api) => true);
});

// Northwind Services
builder.Services.AddNorthwindData(builder.Configuration);
builder.Services.AddNorthwindServices();
builder.Services.AddNorthwindAutoMapper();
builder.Services.AddNorthwindValidation();
builder.Services.AddNorthwindCaching();
builder.Services.AddNorthwindCors();
builder.Services.AddNorthwindApiBehavior();

var app = builder.Build();

// Swagger'ı hem Development hem Production'da çalıştır
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NorthwindApp API v1.0.0");
    c.RoutePrefix = "api-docs";
    c.DocumentTitle = "NorthwindApp API Documentation";
    c.DefaultModelsExpandDepth(2);
    c.DefaultModelExpandDepth(2);
    c.DisplayRequestDuration();
    c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    
    // Custom CSS for better styling
    c.InjectStylesheet("/swagger-ui/custom.css");
    c.InjectJavascript("/swagger-ui/custom.js");
});

app.UseHttpsRedirection();

// Enhanced Serilog Request Logging with business context
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "{RequestMethod} {RequestPath} → {StatusCode} ({Elapsed:0.0000}ms) | {EntityName} | Cache: {CacheStatus} | Records: {RecordCount} | Page: {PageInfo}";
    options.IncludeQueryInRequestPath = true;
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        // Get business context from HTTP context
        var cacheStatus = httpContext.Items.ContainsKey("CacheStatus") ? httpContext.Items["CacheStatus"]?.ToString() ?? "N/A" : "N/A";
        var recordCount = httpContext.Items.ContainsKey("RecordCount") ? httpContext.Items["RecordCount"]?.ToString() ?? "N/A" : "N/A";
        var pageInfo = httpContext.Items.ContainsKey("PageInfo") ? httpContext.Items["PageInfo"]?.ToString() ?? "N/A" : "N/A";
        var entityName = httpContext.Items.ContainsKey("EntityName") ? httpContext.Items["EntityName"]?.ToString() ?? "N/A" : "N/A";
        var cachePrefix = httpContext.Items.ContainsKey("CachePrefix") ? httpContext.Items["CachePrefix"]?.ToString() ?? "N/A" : "N/A";
        
        // Enhance cache status display
        var enhancedCacheStatus = cacheStatus switch
        {
            "HIT" => "HIT (from cache)",
            "MISS" => "MISS (from DB)",
            "DB" => "DB (saved to cache)",
            "EMPTY" => "EMPTY (no results)",
            "ERROR" => "ERROR",
            _ => "N/A"
        };
        
        diagnosticContext.Set("CacheStatus", enhancedCacheStatus);
        diagnosticContext.Set("RecordCount", recordCount);
        diagnosticContext.Set("PageInfo", pageInfo);
        diagnosticContext.Set("EntityName", entityName);
        diagnosticContext.Set("CachePrefix", cachePrefix);
    };
});

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Root endpoint for testing
app.MapGet("/", () => "NorthwindApp API is running!");

app.Run();
