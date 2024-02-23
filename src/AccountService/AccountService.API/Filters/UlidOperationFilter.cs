using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AccountService.API.Filters
{
    public class UlidOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var parameter in operation.Parameters)
            {
                if (parameter.Schema?.Type == "string" && parameter.Schema.Format == "ulid")
                {
                    parameter.Schema.Type = "string";
                    parameter.Schema.Format = null;
                    parameter.Schema.Pattern = @"^[0-9A-Z]{26}$";
                }
            }
        }
    }
}
