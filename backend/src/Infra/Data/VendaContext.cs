using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Infra.Data;
public class VendaContext : DbContext, IUnitOfWork
{
    public VendaContext(DbContextOptions<VendaContext> options) : base(options)
    {
    }
    public DbSet<Venda> Vendas { get; set; }
    public DbSet<VendaItem> VendaItens { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(VendaContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await SaveChangesWithDomainEventsAsync(cancellationToken);
    }
    async Task<int> IUnitOfWork.SaveChangesAsync(CancellationToken cancellationToken)
    {
        return await SaveChangesWithDomainEventsAsync(cancellationToken);
    }
    private async Task<int> SaveChangesWithDomainEventsAsync(CancellationToken cancellationToken)
    {
        var domainEvents = ChangeTracker
            .Entries<Venda>()
            .SelectMany(e => e.Entity.DomainEvents)
            .ToList();
        var result = await base.SaveChangesAsync(cancellationToken);
        foreach (var domainEvent in domainEvents)
        {
            Infra.Logging.DomainEventLogger.LogDomainEvent(domainEvent);
        }
        foreach (var entry in ChangeTracker.Entries<Venda>())
        {
            entry.Entity.ClearDomainEvents();
        }
        return result;
    }
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.BeginTransactionAsync(cancellationToken);
    }
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.CommitTransactionAsync(cancellationToken);
    }
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        await Database.RollbackTransactionAsync(cancellationToken);
    }
}
