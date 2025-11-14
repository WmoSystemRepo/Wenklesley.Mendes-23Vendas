using Domain.Common;
namespace Domain.Events;
public class CompraEfetuada : DomainEvent
{
    public Guid VendaId { get; }
    public string NumeroVenda { get; }
    public decimal ValorTotal { get; }
    public CompraEfetuada(Guid vendaId, string numeroVenda, decimal valorTotal)
    {
        VendaId = vendaId;
        NumeroVenda = numeroVenda;
        ValorTotal = valorTotal;
    }
}
