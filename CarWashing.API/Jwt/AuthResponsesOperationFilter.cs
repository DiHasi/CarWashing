using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CarWashing.Jwt;

public class AuthResponsesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attributes = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
            .Union(context.MethodInfo.GetCustomAttributes(true));

        if (attributes == null) return;

        var attributesList = attributes.ToList();

        Console.WriteLine("Attributes for " + context.MethodInfo.Name + ": " + string.Join(", ", attributesList.Select(a => a.GetType().Name)));

        var authorizeAttributes = attributesList.OfType<AuthorizeAttribute>();

        if (!authorizeAttributes.Any()) return;

        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });

        var needPerms = string.Join(" && ", authorizeAttributes.Select(a => a.Policy).Where(p => !string.IsNullOrEmpty(p)));
        if (!string.IsNullOrEmpty(needPerms))
        {
            operation.Responses.TryAdd("403", new OpenApiResponse { Description = $"Forbidden, need permission \"{needPerms}\"" });
        }

        var securityRequirement = new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "bearerAuth" }
                },
                Array.Empty<string>()
            }
        };

        operation.Security = new List<OpenApiSecurityRequirement> { securityRequirement };
    }
}