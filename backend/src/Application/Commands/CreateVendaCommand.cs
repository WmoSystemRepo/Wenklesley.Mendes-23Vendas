using MediatR;
namespace Application.Commands;
public class CreateVendaCommand : IRequest<Guid>
{
    public string NumeroVenda { get; set; } = string.Empty;
    public Guid ClienteId { get; set; }
    public string ClienteNome { get; set; } = string.Empty;
    public Guid FilialId { get; set; }
    public string FilialNome { get; set; } = string.Empty;
    public List<CreateVendaItemCommand> Itens { get; set; } = new();
}
public class CreateVendaItemCommand
{
    public Guid ProdutoId { get; set; }
    public string ProdutoNome { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
}
