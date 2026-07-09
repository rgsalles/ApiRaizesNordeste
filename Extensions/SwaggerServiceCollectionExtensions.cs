using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.SwaggerUI;
using Microsoft.OpenApi;

namespace ApiRaizesNordeste.Extensions;

/// <summary>
/// Extensões para configuração do Swagger/OpenAPI
/// </summary>
public static class SwaggerServiceCollectionExtensions
{
    /// <summary>
    /// Adiciona os serviços de Swagger e OpenAPI
    /// </summary>
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Informe o token JWT retornado pelo login."
            });
            // Comentários XML
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }

        });

        return services;
    }

    /// <summary>
    /// Configura o middleware do Swagger
    /// </summary>
    public static WebApplication UseSwaggerConfiguration(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "API Raízes Nordeste v1");
                options.RoutePrefix = string.Empty;
            });
        }

        return app;
    }
}
