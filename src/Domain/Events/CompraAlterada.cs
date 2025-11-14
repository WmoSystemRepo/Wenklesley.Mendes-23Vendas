using Domain.Common;
namespace Domain.Events;
public class CompraAlterada : DomainEvent
{
    public Guid VendaId { get; }
    public string NumeroVenda { get; }
    public decimal ValorTotal { get; }
    public CompraAlterada(Guid vendaId, string numeroVenda, decimal valorTotal)
    {
        VendaId = vendaId;
        NumeroVenda = numeroVenda;
        ValorTotal = valorTotal;
    }
}
