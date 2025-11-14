using Application.Commands;
using Application.Interfaces;
using Domain.ValueObjects;
using MediatR;
namespace Application.Handlers;
public class UpdateVendaHandler : IRequestHandler<UpdateVendaCommand>
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IUnitOfWork _unitOfWork;
    public UpdateVendaHandler(IVendaRepository vendaRepository, IUnitOfWork unitOfWork)
    {
        _vendaRepository = vendaRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task Handle(UpdateVendaCommand request, CancellationToken cancellationToken)
    {
        var venda = await _vendaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (venda == null)
            throw new InvalidOperationException("Venda não encontrada");
        if (request.ItensParaAdicionar != null)
        {
            foreach (var item in request.ItensParaAdicionar)
            {
                venda.AdicionarItem(
                    new ProdutoId(item.ProdutoId),
                    item.ProdutoNome,
                    item.Quantidade,
                    item.ValorUnitario);
            }
        }
        if (request.ItensParaRemover != null)
        {
            foreach (var itemId in request.ItensParaRemover)
            {
                venda.RemoverItem(itemId);
            }
        }
        if (request.ItensParaAtualizar != null)
        {
            foreach (var item in request.ItensParaAtualizar)
            {
                venda.AtualizarItem(
                    item.ItemId,
                    item.Quantidade,
                    item.ValorUnitario);
            }
        }
        await _vendaRepository.UpdateAsync(venda, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
