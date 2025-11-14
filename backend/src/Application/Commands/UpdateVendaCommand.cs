using MediatR;
namespace Application.Commands;
public class UpdateVendaCommand : IRequest
{
    public Guid Id { get; set; }
    public List<UpdateVendaItemCommand>? ItensParaAdicionar { get; set; }
    public List<Guid>? ItensParaRemover { get; set; }
    public List<UpdateVendaItemExistenteCommand>? ItensParaAtualizar { get; set; }
}
public class UpdateVendaItemCommand
{
    public Guid ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
}
public class UpdateVendaItemExistenteCommand
{
    public Guid ItemId { get; set; }
    public int? Quantidade { get; set; }
    public decimal? ValorUnitario { get; set; }
}
