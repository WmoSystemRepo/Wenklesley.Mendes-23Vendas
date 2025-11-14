using Application.Commands;
using Application.Interfaces;
using MediatR;
namespace Application.Handlers;
public class DeleteVendaHandler : IRequestHandler<DeleteVendaCommand>
{
    private readonly IVendaRepository _vendaRepository;
    private readonly IUnitOfWork _unitOfWork;
    public DeleteVendaHandler(IVendaRepository vendaRepository, IUnitOfWork unitOfWork)
    {
        _vendaRepository = vendaRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task Handle(DeleteVendaCommand request, CancellationToken cancellationToken)
    {
        var venda = await _vendaRepository.GetByIdAsync(request.Id, cancellationToken);
        if (venda == null)
            throw new InvalidOperationException("Venda não encontrada");
        venda.Cancelar();
        await _vendaRepository.UpdateAsync(venda, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
