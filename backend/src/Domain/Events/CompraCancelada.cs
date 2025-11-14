using Domain.Common;
namespace Domain.Events;
public class CompraCancelada : DomainEvent
{
    public Guid VendaId { get; }
    public string NumeroVenda { get; }
    public CompraCancelada(Guid vendaId, string numeroVenda)
    {
        VendaId = vendaId;
        NumeroVenda = numeroVenda;
    }
}
