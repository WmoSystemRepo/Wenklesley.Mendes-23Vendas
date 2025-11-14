using Application.Commands;
using Application.Handlers;
using Application.Interfaces;
using Domain.Entities;
using Domain.ValueObjects;
using NSubstitute;
using Shouldly;
using Xunit;
namespace UnitTests.Application.Handlers;
public class CreateVendaHandlerTests
{
    [Fact]
    public async Task Handle_DeveCriarVendaComSucesso()
    {
        var repository = Substitute.For<IVendaRepository>();
        var unitOfWork = Substitute.For<IUnitOfWork>();
        var handler = new CreateVendaHandler(repository, unitOfWork);
        var command = new CreateVendaCommand
        {
            NumeroVenda = "V001",
            ClienteId = Guid.NewGuid(),
            ClienteNome = "Cliente Teste",
            FilialId = Guid.NewGuid(),
            FilialNome = "Filial Teste",
            Itens = new List<CreateVendaItemCommand>
            {
                new()
                {
                    ProdutoId = Guid.NewGuid(),
                    ProdutoNome = "Produto Teste",
                    Quantidade = 5,
                    ValorUnitario = 100m
                }
            }
        };
        repository.AddAsync(Arg.Any<Venda>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Venda>());
        unitOfWork.SaveChangesAsync(Arg.Any<CancellationToken>())
            .Returns(Task.FromResult(1));
        var result = await handler.Handle(command, CancellationToken.None);
        result.ShouldNotBe(Guid.Empty);
        await repository.Received(1).AddAsync(Arg.Any<Venda>(), Arg.Any<CancellationToken>());
        await unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
