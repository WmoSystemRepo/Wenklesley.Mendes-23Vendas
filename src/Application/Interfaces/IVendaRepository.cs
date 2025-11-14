using Domain.Entities;
namespace Application.Interfaces;
public interface IVendaRepository
{
    Task<Venda?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Venda>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Venda> AddAsync(Venda venda, CancellationToken cancellationToken = default);
    Task UpdateAsync(Venda venda, CancellationToken cancellationToken = default);
    Task DeleteAsync(Venda venda, CancellationToken cancellationToken = default);
}
