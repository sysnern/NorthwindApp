using FluentValidation;
using Microsoft.AspNetCore.Http;
using NorthwindApp.Core.Results;
using System.Net;
using System.Text.Json;

namespace NorthwindApp.API.Middleware
{
    public class ValidationExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                // 1) HTTP 400 Bad Request
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                // 2) Tüm hata mesajlarını topla
                var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();

                // 3) ApiResponse.BadRequest ile hem Success=false, hem doğru kod, hem Errors listesi
                var response = ApiResponse<string>.BadRequest(
                    errors,
                    message: "Doğrulama hataları oluştu."
                );

                // 4) Camel‑case JSON ayarları
                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var json = JsonSerializer.Serialize(response, jsonOptions);

                await context.Response.WriteAsync(json);
            }
        }
    }
}