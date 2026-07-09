using ApiRaizesNordeste.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiRaizesNordeste.Extensions;

/// <summary>
/// Extensões para configuração de banco de dados
/// </summary>
public static class DatabaseServiceCollectionExtensions
{
    /// <summary>
    /// Adiciona os serviços de banco de dados (Entity Framework + SQL Server)
    /// </summary>
    public static IServiceCollection AddDatabaseServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "A connection string 'DefaultConnection' não foi configurada em appsettings.json");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.GetName().Name);
                sqlOptions.EnableRetryOnFailure();
            }));

        return services;
    }
}
