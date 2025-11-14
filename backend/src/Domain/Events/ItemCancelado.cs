using Domain.Common;
namespace Domain.Events;
public class ItemCancelado : DomainEvent
{
    public Guid VendaId { get; }
    public Guid ItemId { get; }
    public string ProdutoNome { get; }
    public ItemCancelado(Guid vendaId, Guid itemId, string produtoNome)
    {
        VendaId = vendaId;
        ItemId = itemId;
        ProdutoNome = produtoNome;
    }
}
