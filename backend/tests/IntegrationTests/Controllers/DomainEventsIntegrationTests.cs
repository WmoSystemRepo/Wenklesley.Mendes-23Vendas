using Application.Commands;
using Application.DTOs;
using IntegrationTests.Fixtures;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;
namespace IntegrationTests.Controllers;
public class DomainEventsIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;
    public DomainEventsIntegrationTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task Post_CriarVenda_DeveEmitirEventoCompraEfetuada()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        getResponse.Data!.NumeroVenda.ShouldBe(command.NumeroVenda);
    }
    [Fact]
    public async Task Post_CriarVenda_DeveCriarVendaComValorTotalCorreto()
    {
        var command = VendaTestHelper.CriarVendaCommandValida(quantidadeItens: 1, quantidadePorItem: 5, valorUnitario: 100m);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        getResponse.Data!.ValorTotal.ShouldBe(450m);
    }
    [Fact]
    public async Task Put_AtualizarVenda_DeveEmitirEventoCompraAlterada()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponseAntes = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponseAntes.Success.ShouldBeTrue();
        getResponseAntes.Data.ShouldNotBeNull();
        var itemId = getResponseAntes.Data!.Itens.First().Id;
        var updateCommand = new UpdateVendaCommand
        {
            Id = vendaId,
            ItensParaAtualizar = new List<UpdateVendaItemExistenteCommand>
            {
                new() { ItemId = itemId, Quantidade = 10 }
            }
        };
        var updateResponse = await _client.PutAsync($"/api/venda/{vendaId}", updateCommand);
        updateResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
        var getResponseDepois = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponseDepois.Success.ShouldBeTrue();
        getResponseDepois.Data.ShouldNotBeNull();
        getResponseDepois.Data!.ValorTotal.ShouldNotBe(getResponseAntes.Data!.ValorTotal);
    }
    [Fact]
    public async Task Put_AtualizarVenda_DeveRecalcularValorTotalNoEvento()
    {
        var command = VendaTestHelper.CriarVendaCommandValida(quantidadeItens: 1, quantidadePorItem: 5, valorUnitario: 100m);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponseAntes = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponseAntes.Success.ShouldBeTrue();
        getResponseAntes.Data.ShouldNotBeNull();
        var itemId = getResponseAntes.Data!.Itens.First().Id;
        var updateCommand = new UpdateVendaCommand
        {
            Id = vendaId,
            ItensParaAtualizar = new List<UpdateVendaItemExistenteCommand>
            {
                new() { ItemId = itemId, Quantidade = 15 }
            }
        };
        await _client.PutAsync($"/api/venda/{vendaId}", updateCommand);
        var getResponseDepois = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponseDepois.Success.ShouldBeTrue();
        getResponseDepois.Data.ShouldNotBeNull();
        getResponseDepois.Data!.ValorTotal.ShouldBe(1200m);
    }
    [Fact]
    public async Task Delete_CancelarVenda_DeveEmitirEventoCompraCancelada()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var deleteResponse = await _client.DeleteAsync($"/api/venda/{vendaId}");
        deleteResponse.StatusCode.ShouldBe(System.Net.HttpStatusCode.NoContent);
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        getResponse.Data!.Status.ShouldBe("Cancelado");
    }
    [Fact]
    public async Task Put_RemoverItem_DeveEmitirEventoItemCancelado()
    {
        var command = VendaTestHelper.CriarVendaCommandValida(quantidadeItens: 2);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponseAntes = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponseAntes.Success.ShouldBeTrue();
        getResponseAntes.Data.ShouldNotBeNull();
        var itemId = getResponseAntes.Data!.Itens.First().Id;
        var quantidadeItensAntes = getResponseAntes.Data.Itens.Count;
        var updateCommand = new UpdateVendaCommand
        {
            Id = vendaId,
            ItensParaRemover = new List<Guid> { itemId }
        };
        await _client.PutAsync($"/api/venda/{vendaId}", updateCommand);
        var getResponseDepois = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponseDepois.Success.ShouldBeTrue();
        getResponseDepois.Data.ShouldNotBeNull();
        getResponseDepois.Data!.Itens.Count.ShouldBe(quantidadeItensAntes - 1);
    }
    [Fact]
    public async Task Put_RemoverItem_DeveRecalcularValorTotal()
    {
        var command = VendaTestHelper.CriarVendaCommandValida(quantidadeItens: 2, quantidadePorItem: 5, valorUnitario: 100m);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponseAntes = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponseAntes.Success.ShouldBeTrue();
        getResponseAntes.Data.ShouldNotBeNull();
        var itemId = getResponseAntes.Data!.Itens.First().Id;
        var valorTotalAntes = getResponseAntes.Data.ValorTotal;
        var updateCommand = new UpdateVendaCommand
        {
            Id = vendaId,
            ItensParaRemover = new List<Guid> { itemId }
        };
        await _client.PutAsync($"/api/venda/{vendaId}", updateCommand);
        var getResponseDepois = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponseDepois.Success.ShouldBeTrue();
        getResponseDepois.Data.ShouldNotBeNull();
        getResponseDepois.Data!.ValorTotal.ShouldBeLessThan(valorTotalAntes);
    }
}
