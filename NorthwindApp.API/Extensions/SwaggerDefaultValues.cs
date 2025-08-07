using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NorthwindApp.API.Extensions
{
    /// <summary>
    /// Represents the Swagger/Swashbuckle operation filter for enhanced API documentation
    /// </summary>
    public class SwaggerDefaultValues : IOperationFilter
    {
        /// <summary>
        /// Applies the filter to the specified operation using the given context.
        /// </summary>
        /// <param name="operation">The operation to apply the filter to.</param>
        /// <param name="context">The current operation filter context.</param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            // Add operation ID if not present
            if (string.IsNullOrEmpty(operation.OperationId))
            {
                operation.OperationId = $"{context.MethodInfo.DeclaringType?.Name}_{context.MethodInfo.Name}";
            }

            // Add tags based on controller name
            if (operation.Tags == null || !operation.Tags.Any())
            {
                var controllerName = context.MethodInfo.DeclaringType?.Name?.Replace("Controller", "");
                if (!string.IsNullOrEmpty(controllerName))
                {
                    operation.Tags = new List<OpenApiTag> { new OpenApiTag { Name = controllerName } };
                }
            }

            // Add summary if not present
            if (string.IsNullOrEmpty(operation.Summary))
            {
                operation.Summary = $"{context.MethodInfo.Name} operation";
            }

            // Add description if not present
            if (string.IsNullOrEmpty(operation.Description))
            {
                operation.Description = $"Performs {context.MethodInfo.Name.ToLower()} operation";
            }

            // Ensure responses are properly documented
            if (operation.Responses == null)
            {
                operation.Responses = new OpenApiResponses();
            }

            // Add default responses if not present
            if (!operation.Responses.ContainsKey("200"))
            {
                operation.Responses.Add("200", new OpenApiResponse
                {
                    Description = "Success",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema { Type = "object" }
                        }
                    }
                });
            }

            if (!operation.Responses.ContainsKey("400"))
            {
                operation.Responses.Add("400", new OpenApiResponse
                {
                    Description = "Bad Request",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema { Type = "object" }
                        }
                    }
                });
            }

            if (!operation.Responses.ContainsKey("404"))
            {
                operation.Responses.Add("404", new OpenApiResponse
                {
                    Description = "Not Found",
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["application/json"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema { Type = "object" }
                        }
                    }
                });
            }
        }
    }
}
