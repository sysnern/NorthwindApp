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
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                context.Response.ContentType = "application/json";

                var message = string.Join(", ", ex.Errors.Select(e => e.ErrorMessage));
                var response = ApiResponse<string>.Fail(message);

                var json = JsonSerializer.Serialize(response);

                await context.Response.WriteAsync(json);
            }
        }
    }
}
