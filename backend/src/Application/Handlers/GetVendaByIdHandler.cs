using Application.DTOs;
using Application.Interfaces;
using Application.Queries;
using MediatR;
namespace Application.Handlers;
public class GetVendaByIdHandler : IRequestHandler<GetVendaByIdQuery, VendaDto?>
{
    private readonly IVendaRepository _vendaRepository;
    public GetVendaByIdHandler(IVendaRepository vendaRepository)
    {
        _vendaRepository = vendaRepository;
    }
    public async Task<VendaDto?> Handle(GetVendaByIdQuery request, CancellationToken cancellationToken)
    {
        var venda = await _vendaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (venda == null)
            return null;
        return MapToDto(venda);
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
