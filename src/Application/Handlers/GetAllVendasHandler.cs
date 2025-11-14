using Application.DTOs;
using Application.Interfaces;
using Application.Queries;
using MediatR;
namespace Application.Handlers;
public class GetAllVendasHandler : IRequestHandler<GetAllVendasQuery, IEnumerable<VendaDto>>
{
    private readonly IVendaRepository _vendaRepository;
    public GetAllVendasHandler(IVendaRepository vendaRepository)
    {
        _vendaRepository = vendaRepository;
    }
    public async Task<IEnumerable<VendaDto>> Handle(GetAllVendasQuery request, CancellationToken cancellationToken)
    {
        var vendas = await _vendaRepository.GetAllAsync(cancellationToken);
        return vendas.Select(MapToDto);
    }
    private static VendaDto MapToDto(Domain.Entities.Venda venda)
    {
        return new VendaDto
        {
            Id = venda.Id,
            NumeroVenda = venda.NumeroVenda,
            Data = venda.Data,
            ClienteId = venda.ClienteId.Value,
            ClienteNome = venda.ClienteNome,
            FilialId = venda.FilialId.Value,
            FilialNome = venda.FilialNome,
            Status = venda.Status.ToString(),
            ValorTotal = venda.ValorTotal,
            Itens = venda.Itens.Select(i => new VendaItemDto
            {
                Id = i.Id,
                ProdutoId = i.ProdutoId.Value,
                ProdutoNome = i.ProdutoNome,
                Quantidade = i.Quantidade,
                ValorUnitario = i.ValorUnitario,
                Desconto = i.Desconto,
                ValorTotalItem = i.ValorTotalItem
            }).ToList()
        };
    }
}
