using Application.DTOs;
using MediatR;
namespace Application.Queries;
public class GetAllVendasQuery : IRequest<IEnumerable<VendaDto>>
{
}
