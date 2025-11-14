using Api.Middlewares;
using FluentValidation;
using Infra.IoC;
using Infra.Logging;
using Microsoft.EntityFrameworkCore;
using Serilog;
var builder = WebApplication.CreateBuilder(args);
SerilogConfiguration.ConfigureSerilog();
builder.Host.UseSerilog();
builder.Services.AddControllers();
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        }
        else
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        }
    });
});
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Title = "123Vendas API",
            Version = "v1",
            Description = "API RESTful para gerenciamento de vendas desenvolvida com Clean Architecture e DDD",
            Contact = new Microsoft.OpenApi.Models.OpenApiContact
            {
                Name = "123Vendas",
                Email = "contato@123vendas.com"
            }
        });
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            c.IncludeXmlComments(xmlPath);
        }
        c.UseInlineDefinitionsForEnums();
    });
}
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);
builder.Services.AddMediatR(cfg => 
{
    cfg.RegisterServicesFromAssembly(typeof(Application.Handlers.CreateVendaHandler).Assembly);
});
builder.Services.AddValidatorsFromAssembly(typeof(Application.Validators.CreateVendaCommandValidator).Assembly);
var app = builder.Build();
if (!app.Environment.IsEnvironment("Test"))
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<Infra.Data.VendaContext>();
        try
        {
            context.Database.Migrate();
        }
        catch { }
    }
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "123Vendas API V1");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
    });
}
app.UseMiddleware<PerformanceMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapGet("/health", () =>
{
    var health = new
    {
        status = "healthy",
        timestamp = DateTime.UtcNow,
        uptime = Environment.TickCount64 / 1000,
        environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
        machineName = Environment.MachineName,
        version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString()
    };
    return Results.Ok(health);
}).ExcludeFromDescription();
app.MapControllers();
app.Run();
public partial class Program { }
