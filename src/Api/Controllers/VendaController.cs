using Api.Models;
using Application.Commands;
using Application.DTOs;
using Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace Api.Controllers;
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class VendaController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<VendaController> _logger;
    public VendaController(IMediator mediator, ILogger<VendaController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<VendaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var query = new GetAllVendasQuery();
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(ApiResponse<IEnumerable<VendaDto>>.SuccessResponse(result, correlationId: correlationId));
    }
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<VendaDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var query = new GetVendaByIdQuery { Id = id };
        var result = await _mediator.Send(query, cancellationToken);
        if (result == null)
            return NotFound(ApiResponse<object>.ErrorResponse("Venda não encontrada", correlationId: correlationId));
        return Ok(ApiResponse<VendaDto>.SuccessResponse(result, correlationId: correlationId));
    }
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateVendaCommand command, 
        CancellationToken cancellationToken)
    {
        var correlationId = HttpContext.Items["CorrelationId"]?.ToString();
        var id = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(
            nameof(GetById), 
            new { id }, 
            ApiResponse<Guid>.SuccessResponse(id, "Venda criada com sucesso", correlationId));
    }
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id, 
        [FromBody] UpdateVendaCommand command, 
        CancellationToken cancellationToken)
    {
        command.Id = id;
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id, 
        CancellationToken cancellationToken)
    {
        var command = new DeleteVendaCommand { Id = id };
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
