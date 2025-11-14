using Application.Commands;
using Application.DTOs;
using IntegrationTests.Fixtures;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;
namespace IntegrationTests.Controllers;
public class RegrasDeNegocioIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;
    public RegrasDeNegocioIntegrationTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }
    #region Regra 1: Desconto 10% para > 4 itens (até 9)
    [Fact]
    public async Task Post_Com5Itens_DeveAplicar10PorcentoDesconto()
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidade(5);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        var item = getResponse.Data!.Itens.First();
        item.Desconto.ShouldBe(50m);
        item.ValorTotalItem.ShouldBe(450m);
    }
    [Fact]
    public async Task Post_Com9Itens_DeveAplicar10PorcentoDesconto()
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidade(9);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        var item = getResponse.Data!.Itens.First();
        item.Desconto.ShouldBe(90m);
        item.ValorTotalItem.ShouldBe(810m);
    }
    [Fact]
    public async Task Post_Com4Itens_NaoDeveAplicarDesconto()
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidade(4);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        var item = getResponse.Data!.Itens.First();
        item.Desconto.ShouldBe(0m);
        item.ValorTotalItem.ShouldBe(400m);
    }
    [Fact]
    public async Task Put_AtualizandoPara5Itens_DeveAplicar10PorcentoDesconto()
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidade(4);
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
                new() { ItemId = itemId, Quantidade = 5 }
            }
        };
        await _client.PutAsync($"/api/venda/{vendaId}", updateCommand);
        var getResponseDepois = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponseDepois.Success.ShouldBeTrue();
        getResponseDepois.Data.ShouldNotBeNull();
        var item = getResponseDepois.Data!.Itens.First();
        item.Desconto.ShouldBe(50m);
        item.ValorTotalItem.ShouldBe(450m);
    }
    #endregion
    #region Regra 2: Desconto 20% para 10-20 itens
    [Fact]
    public async Task Post_Com10Itens_DeveAplicar20PorcentoDesconto()
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidade(10);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        var item = getResponse.Data!.Itens.First();
        item.Desconto.ShouldBe(200m);
        item.ValorTotalItem.ShouldBe(800m);
    }
    [Fact]
    public async Task Post_Com15Itens_DeveAplicar20PorcentoDesconto()
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidade(15);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        var item = getResponse.Data!.Itens.First();
        item.Desconto.ShouldBe(300m);
        item.ValorTotalItem.ShouldBe(1200m);
    }
    [Fact]
    public async Task Post_Com20Itens_DeveAplicar20PorcentoDesconto()
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidade(20);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        var item = getResponse.Data!.Itens.First();
        item.Desconto.ShouldBe(400m);
        item.ValorTotalItem.ShouldBe(1600m);
    }
    [Fact]
    public async Task Post_Com9Itens_DeveAplicar10PorcentoNao20()
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidade(9);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        var item = getResponse.Data!.Itens.First();
        item.Desconto.ShouldBe(90m);
        item.ValorTotalItem.ShouldBe(810m);
    }
    #endregion
    #region Regra 3: Não permitir > 20 itens
    [Fact]
    public async Task Post_Com21Itens_DeveRetornar400BadRequest()
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidadeInvalida(21);
        var response = await _client.PostAsync("/api/venda", command);
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task Put_AtualizandoPara21Itens_DeveRetornar400BadRequest()
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidade(5);
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
                new() { ItemId = itemId, Quantidade = 21 }
            }
        };
        var response = await _client.PutAsync($"/api/venda/{vendaId}", updateCommand);
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task Post_Com20Itens_DevePermitir()
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidade(20);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        createResponse.Data.ShouldNotBe(Guid.Empty);
    }
    #endregion
    #region Regra 4: Não permitir desconto < 4 itens
    [Fact]
    public async Task Post_Com1Item_NaoDeveTerDesconto()
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidade(1);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        var item = getResponse.Data!.Itens.First();
        item.Desconto.ShouldBe(0m);
        item.ValorTotalItem.ShouldBe(100m);
    }
    [Fact]
    public async Task Post_Com2Itens_NaoDeveTerDesconto()
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidade(2);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        var item = getResponse.Data!.Itens.First();
        item.Desconto.ShouldBe(0m);
        item.ValorTotalItem.ShouldBe(200m);
    }
    [Fact]
    public async Task Post_Com3Itens_NaoDeveTerDesconto()
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidade(3);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        var item = getResponse.Data!.Itens.First();
        item.Desconto.ShouldBe(0m);
        item.ValorTotalItem.ShouldBe(300m);
    }
    [Fact]
    public async Task Put_AtualizandoPara3Itens_NaoDeveTerDesconto()
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidade(5);
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
                new() { ItemId = itemId, Quantidade = 3 }
            }
        };
        await _client.PutAsync($"/api/venda/{vendaId}", updateCommand);
        var getResponseDepois = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponseDepois.Success.ShouldBeTrue();
        getResponseDepois.Data.ShouldNotBeNull();
        var item = getResponseDepois.Data!.Itens.First();
        item.Desconto.ShouldBe(0m);
        item.ValorTotalItem.ShouldBe(300m);
    }
    #endregion
}
