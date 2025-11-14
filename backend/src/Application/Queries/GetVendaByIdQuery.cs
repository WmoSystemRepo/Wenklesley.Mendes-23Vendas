using Application.DTOs;
using MediatR;
namespace Application.Queries;
public class GetVendaByIdQuery : IRequest<VendaDto?>
{
    public Guid Id { get; set; }
}
