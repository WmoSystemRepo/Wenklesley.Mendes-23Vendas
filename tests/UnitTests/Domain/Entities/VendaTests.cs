using Domain.Entities;
using Domain.Enums;
using Domain.Events;
using Domain.ValueObjects;
using Shouldly;
using Xunit;
namespace UnitTests.Domain.Entities;
public class VendaTests
{
    [Fact]
    public void CriarVenda_DeveCriarVendaComStatusNaoCancelado()
    {
        var clienteId = new ClienteId(Guid.NewGuid());
        var filialId = new FilialId(Guid.NewGuid());
        var venda = new Venda("V001", clienteId, "Cliente Teste", filialId, "Filial Teste");
        venda.NumeroVenda.ShouldBe("V001");
        venda.ClienteId.ShouldBe(clienteId);
        venda.ClienteNome.ShouldBe("Cliente Teste");
        venda.FilialId.ShouldBe(filialId);
        venda.FilialNome.ShouldBe("Filial Teste");
        venda.Status.ShouldBe(VendaStatus.NaoCancelado);
        venda.ValorTotal.ShouldBe(0);
        venda.Itens.ShouldBeEmpty();
    }
    [Fact]
    public void AdicionarItem_DeveAdicionarItemEVincularAVenda()
    {
        var venda = CriarVenda();
        var produtoId = new ProdutoId(Guid.NewGuid());
        venda.AdicionarItem(produtoId, "Produto Teste", 5, 100m);
        venda.Itens.ShouldHaveSingleItem();
        var item = venda.Itens.First();
        item.ProdutoId.ShouldBe(produtoId);
        item.ProdutoNome.ShouldBe("Produto Teste");
        item.Quantidade.ShouldBe(5);
        item.ValorUnitario.ShouldBe(100m);
    }
    [Fact]
    public void AdicionarItem_DeveRecalcularValorTotal()
    {
        var venda = CriarVenda();
        var produtoId = new ProdutoId(Guid.NewGuid());
        venda.AdicionarItem(produtoId, "Produto Teste", 5, 100m);
        venda.ValorTotal.ShouldBe(450m);
    }
    [Fact]
    public void AdicionarItem_DeveEmitirEventoCompraEfetuada()
    {
        var venda = CriarVenda();
        var produtoId = new ProdutoId(Guid.NewGuid());
        venda.AdicionarItem(produtoId, "Produto Teste", 5, 100m);
        venda.DomainEvents.ShouldContain(e => e is CompraEfetuada);
    }
    [Fact]
    public void AdicionarItem_VendaCancelada_DeveLancarExcecao()
    {
        var venda = CriarVenda();
        venda.Cancelar();
        var produtoId = new ProdutoId(Guid.NewGuid());
        Should.Throw<InvalidOperationException>(() =>
            venda.AdicionarItem(produtoId, "Produto Teste", 5, 100m));
    }
    [Fact]
    public void RemoverItem_DeveRemoverItemEVincularAVenda()
    {
        var venda = CriarVenda();
        var produtoId = new ProdutoId(Guid.NewGuid());
        venda.AdicionarItem(produtoId, "Produto Teste", 5, 100m);
        var itemId = venda.Itens.First().Id;
        venda.RemoverItem(itemId);
        venda.Itens.ShouldBeEmpty();
    }
    [Fact]
    public void RemoverItem_DeveEmitirEventoItemCancelado()
    {
        var venda = CriarVenda();
        var produtoId = new ProdutoId(Guid.NewGuid());
        venda.AdicionarItem(produtoId, "Produto Teste", 5, 100m);
        var itemId = venda.Itens.First().Id;
        venda.ClearDomainEvents();
        venda.RemoverItem(itemId);
        venda.DomainEvents.ShouldContain(e => e is ItemCancelado);
    }
    [Fact]
    public void AtualizarItem_DeveAtualizarQuantidadeERecalcularValor()
    {
        var venda = CriarVenda();
        var produtoId = new ProdutoId(Guid.NewGuid());
        venda.AdicionarItem(produtoId, "Produto Teste", 5, 100m);
        var itemId = venda.Itens.First().Id;
        venda.ClearDomainEvents();
        venda.AtualizarItem(itemId, quantidade: 10);
        var item = venda.Itens.First();
        item.Quantidade.ShouldBe(10);
        item.Desconto.ShouldBe(200m);
        venda.DomainEvents.ShouldContain(e => e is CompraAlterada);
    }
    [Fact]
    public void Cancelar_DeveAlterarStatusParaCancelado()
    {
        var venda = CriarVenda();
        venda.Cancelar();
        venda.Status.ShouldBe(VendaStatus.Cancelado);
    }
    [Fact]
    public void Cancelar_DeveEmitirEventoCompraCancelada()
    {
        var venda = CriarVenda();
        venda.Cancelar();
        venda.DomainEvents.ShouldContain(e => e is CompraCancelada);
    }
    [Fact]
    public void Cancelar_VendaJaCancelada_DeveLancarExcecao()
    {
        var venda = CriarVenda();
        venda.Cancelar();
        Should.Throw<InvalidOperationException>(() => venda.Cancelar());
    }
    private static Venda CriarVenda()
    {
        return new Venda(
            "V001",
            new ClienteId(Guid.NewGuid()),
            "Cliente Teste",
            new FilialId(Guid.NewGuid()),
            "Filial Teste");
    }
}
