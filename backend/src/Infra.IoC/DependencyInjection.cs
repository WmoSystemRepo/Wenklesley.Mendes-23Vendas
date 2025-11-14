using Application.Interfaces;
using Infra.Data;
using Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace Infra.IoC;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, object? environment = null)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var isDevelopment = false;
        if (environment != null)
        {
            var envType = environment.GetType();
            var isDevelopmentMethod = envType.GetMethod("IsDevelopment");
            if (isDevelopmentMethod != null)
            {
                isDevelopment = (bool)(isDevelopmentMethod.Invoke(environment, null) ?? false);
            }
        }
        services.AddDbContext<VendaContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });
            if (isDevelopment)
            {
                options.EnableSensitiveDataLogging();
            }
        });
        services.AddScoped<IVendaRepository, VendaRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<VendaContext>());
        return services;
    }
}
