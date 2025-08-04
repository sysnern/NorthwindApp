using NorthwindApp.API.Extensions;
using NorthwindApp.API.Middleware;
using NorthwindApp.Data.Extensions;
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
    .CreateLogger();

builder.Host.UseSerilog();                            

//service registrations(DI)
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Northwind Services
builder.Services.AddNorthwindData(builder.Configuration);
builder.Services.AddNorthwindServices();
builder.Services.AddNorthwindAutoMapper();
builder.Services.AddNorthwindValidation();
builder.Services.AddNorthwindCaching();
builder.Services.AddNorthwindCors();
builder.Services.AddNorthwindApiBehavior();

var app = builder.Build();

if (app.Environment.IsDevelopment()) 
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<ValidationExceptionMiddleware>();

app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();
app.Run();
