using Application.Commands;
using Application.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using MediatR;
namespace Application.Handlers;
public class CreateVendaHandler : IRequestHandler<CreateVendaCommand, Guid>
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateVendaHandler(IVendaRepository vendaRepository, IUnitOfWork unitOfWork)
    {
        _vendaRepository = vendaRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Guid> Handle(CreateVendaCommand request, CancellationToken cancellationToken)
    {
        var venda = new Venda(
            request.NumeroVenda,
            new ClienteId(request.ClienteId),
            request.ClienteNome,
            new FilialId(request.FilialId),
            request.FilialNome);
        foreach (var item in request.Itens)
        {
            venda.AdicionarItem(
                new ProdutoId(item.ProdutoId),
                item.ProdutoNome,
                item.Quantidade,
                item.ValorUnitario);
        }
        await _vendaRepository.AddAsync(venda, cancellationToken);
        var saved = await _unitOfWork.SaveChangesAsync(cancellationToken);
        if (saved == 0)
            throw new InvalidOperationException("Falha ao salvar venda no banco de dados");
        return venda.Id;
    }
}
