using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Infra.Data;
namespace IntegrationTests.Fixtures;
public class ApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private static readonly string _connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
        ?? "Server=localhost,1434;Database=123Vendas_Test;User Id=sa;Password=Test@Passw0rd123;TrustServerCertificate=True;MultipleActiveResultSets=true;";
    private static readonly object _lock = new();
    private static bool _databaseInitialized = false;
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<VendaContext>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            var contextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(VendaContext));
            if (contextDescriptor != null)
            {
                services.Remove(contextDescriptor);
            }
            services.AddDbContext<VendaContext>(options =>
            {
                options.UseSqlServer(_connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 3,
                        maxRetryDelay: TimeSpan.FromSeconds(5),
                        errorNumbersToAdd: null);
                });
                options.EnableSensitiveDataLogging();
            });
            var repoDescriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(Application.Interfaces.IVendaRepository));
            if (repoDescriptor == null)
            {
                services.AddScoped<Application.Interfaces.IVendaRepository, Infra.Repositories.VendaRepository>();
            }
            var uowDescriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(Application.Interfaces.IUnitOfWork));
            if (uowDescriptor == null)
            {
                services.AddScoped<Application.Interfaces.IUnitOfWork>(sp => sp.GetRequiredService<VendaContext>());
            }
        });
        builder.UseEnvironment("Test");
    }
    public new HttpClient CreateClient()
    {
        var client = base.CreateClient();
        InitializeDatabase();
        return client;
    }
    private void InitializeDatabase()
    {
        if (_databaseInitialized)
            return;
        lock (_lock)
        {
            if (_databaseInitialized)
                return;
            try
            {
                using (var scope = Services.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<VendaContext>();
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }
                _databaseInitialized = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao inicializar banco: {ex.Message}");
            }
        }
    }
    public void CleanDatabase()
    {
        lock (_lock)
        {
            using var scope = Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<VendaContext>();
            var vendas = context.Vendas.Include(v => v.Itens).ToList();
            foreach (var venda in vendas)
            {
                context.VendaItens.RemoveRange(venda.Itens);
            }
            context.Vendas.RemoveRange(vendas);
            context.SaveChanges();
        }
    }
}
