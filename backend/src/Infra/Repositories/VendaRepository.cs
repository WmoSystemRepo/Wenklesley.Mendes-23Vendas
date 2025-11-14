using Application.Interfaces;
using Domain.Entities;
using Infra.Data;
using Microsoft.EntityFrameworkCore;
namespace Infra.Repositories;
public class VendaRepository : IVendaRepository
{
    private readonly VendaContext _context;
    public VendaRepository(VendaContext context)
    {
        _context = context;
    }
    public async Task<Venda?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Vendas
            .Include(v => v.Itens)
            .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);
    }
    public async Task<IEnumerable<Venda>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Vendas
            .Include(v => v.Itens)
            .ToListAsync(cancellationToken);
    }
    public async Task<Venda> AddAsync(Venda venda, CancellationToken cancellationToken = default)
    {
        await _context.Vendas.AddAsync(venda, cancellationToken);
        return venda;
    }
    public Task UpdateAsync(Venda venda, CancellationToken cancellationToken = default)
    {
        var entry = _context.Entry(venda);
        if (entry.State == Microsoft.EntityFrameworkCore.EntityState.Detached)
        {
            _context.Vendas.Update(venda);
        }
        else
        {
            var trackedItens = _context.ChangeTracker.Entries<VendaItem>()
                .Where(e => e.Entity.VendaId == venda.Id && e.State != Microsoft.EntityFrameworkCore.EntityState.Deleted)
                .Select(e => e.Entity.Id)
                .ToList();
            var currentItens = venda.Itens.Select(i => i.Id).ToList();
            var itensToRemove = trackedItens.Except(currentItens).ToList();
            foreach (var itemId in itensToRemove)
            {
                var itemEntry = _context.ChangeTracker.Entries<VendaItem>()
                    .FirstOrDefault(e => e.Entity.Id == itemId);
                if (itemEntry != null)
                {
                    _context.VendaItens.Remove(itemEntry.Entity);
                }
            }
            foreach (var item in venda.Itens)
            {
                var itemEntry = _context.Entry(item);
                if (itemEntry.State == Microsoft.EntityFrameworkCore.EntityState.Detached)
                {
                    _context.VendaItens.Add(item);
                }
            }
            _context.ChangeTracker.DetectChanges();
        }
        return Task.CompletedTask;
    }
    public Task DeleteAsync(Venda venda, CancellationToken cancellationToken = default)
    {
        _context.Vendas.Remove(venda);
        return Task.CompletedTask;
    }
}
