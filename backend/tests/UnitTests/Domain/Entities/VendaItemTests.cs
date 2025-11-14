using Domain.Entities;
using Domain.ValueObjects;
using Shouldly;
using Xunit;
namespace UnitTests.Domain.Entities;
public class VendaItemTests
{
    [Fact]
    public void CriarItem_QuantidadeAte3_DeveAplicarSemDesconto()
    {
        var item = new VendaItem(
            Guid.NewGuid(),
            new ProdutoId(Guid.NewGuid()),
            "Produto Teste",
            3,
            100m);
        item.Quantidade.ShouldBe(3);
        item.ValorUnitario.ShouldBe(100m);
        item.Desconto.ShouldBe(0);
        item.ValorTotalItem.ShouldBe(300m);
    }
    [Fact]
    public void CriarItem_Quantidade4_DeveAplicarSemDesconto()
    {
        var item = new VendaItem(
            Guid.NewGuid(),
            new ProdutoId(Guid.NewGuid()),
            "Produto Teste",
            4,
            100m);
        item.Quantidade.ShouldBe(4);
        item.ValorUnitario.ShouldBe(100m);
        item.Desconto.ShouldBe(0);
        item.ValorTotalItem.ShouldBe(400m);
    }
    [Fact]
    public void CriarItem_QuantidadeEntre5E9_DeveAplicar10PorcentoDesconto()
    {
        var item = new VendaItem(
            Guid.NewGuid(),
            new ProdutoId(Guid.NewGuid()),
            "Produto Teste",
            5,
            100m);
        item.Quantidade.ShouldBe(5);
        item.ValorUnitario.ShouldBe(100m);
        item.Desconto.ShouldBe(50m);
        item.ValorTotalItem.ShouldBe(450m);
    }
    [Fact]
    public void CriarItem_QuantidadeEntre10E20_DeveAplicar20PorcentoDesconto()
    {
        var item = new VendaItem(
            Guid.NewGuid(),
            new ProdutoId(Guid.NewGuid()),
            "Produto Teste",
            15,
            100m);
        item.Quantidade.ShouldBe(15);
        item.ValorUnitario.ShouldBe(100m);
        item.Desconto.ShouldBe(300m);
        item.ValorTotalItem.ShouldBe(1200m);
    }
    [Fact]
    public void CriarItem_QuantidadeMaiorQue20_DeveLancarExcecao()
    {
        Should.Throw<InvalidOperationException>(() =>
            new VendaItem(
                Guid.NewGuid(),
                new ProdutoId(Guid.NewGuid()),
                "Produto Teste",
                21,
                100m));
    }
    [Fact]
    public void CriarItem_QuantidadeZero_DeveLancarExcecao()
    {
        Should.Throw<ArgumentException>(() =>
            new VendaItem(
                Guid.NewGuid(),
                new ProdutoId(Guid.NewGuid()),
                "Produto Teste",
                0,
                100m));
    }
    [Fact]
    public void CriarItem_ValorUnitarioZero_DeveLancarExcecao()
    {
        Should.Throw<ArgumentException>(() =>
            new VendaItem(
                Guid.NewGuid(),
                new ProdutoId(Guid.NewGuid()),
                "Produto Teste",
                5,
                0m));
    }
    [Fact]
    public void AtualizarQuantidade_DeveRecalcularDesconto()
    {
        var item = new VendaItem(
            Guid.NewGuid(),
            new ProdutoId(Guid.NewGuid()),
            "Produto Teste",
            3,
            100m);
        item.AtualizarQuantidade(10);
        item.Quantidade.ShouldBe(10);
        item.Desconto.ShouldBe(200m);
        item.ValorTotalItem.ShouldBe(800m);
    }
    [Fact]
    public void AtualizarQuantidade_MaiorQue20_DeveLancarExcecao()
    {
        var item = new VendaItem(
            Guid.NewGuid(),
            new ProdutoId(Guid.NewGuid()),
            "Produto Teste",
            5,
            100m);
        Should.Throw<InvalidOperationException>(() => item.AtualizarQuantidade(21));
    }
    [Fact]
    public void AtualizarValorUnitario_DeveRecalcularValorTotal()
    {
        var item = new VendaItem(
            Guid.NewGuid(),
            new ProdutoId(Guid.NewGuid()),
            "Produto Teste",
            5,
            100m);
        item.AtualizarValorUnitario(200m);
        item.ValorUnitario.ShouldBe(200m);
        item.Desconto.ShouldBe(100m);
        item.ValorTotalItem.ShouldBe(900m);
    }
}
