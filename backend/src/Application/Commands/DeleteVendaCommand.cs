using MediatR;
namespace Application.Commands;
public class DeleteVendaCommand : IRequest
{
    public Guid Id { get; set; }
}
