using System.Net;
using Application.Commands;
using Application.DTOs;
using Api.Models;
using IntegrationTests.Fixtures;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;
namespace IntegrationTests.Controllers;
public class VendaControllerIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ApiWebApplicationFactory _factory;
    public VendaControllerIntegrationTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }
    #region CREATE (POST /api/venda)
    [Fact]
    public async Task Post_CriarVenda_DeveRetornar201CreatedComId()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        var response = await _client.PostAsync<Guid>("/api/venda", command);
        response.Success.ShouldBeTrue();
        response.Data.ShouldNotBe(Guid.Empty);
    }
    [Fact]
    public async Task Post_CriarVenda_DeveCriarComTodosOsCamposObrigatorios()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        var clienteId = command.ClienteId;
        var filialId = command.FilialId;
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        createResponse.Data.ShouldNotBe(Guid.Empty);
        var vendaId = createResponse.Data;
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        getResponse.Data!.NumeroVenda.ShouldBe(command.NumeroVenda);
        getResponse.Data.ClienteId.ShouldBe(clienteId);
        getResponse.Data.ClienteNome.ShouldBe(command.ClienteNome);
        getResponse.Data.FilialId.ShouldBe(filialId);
        getResponse.Data.FilialNome.ShouldBe(command.FilialNome);
        getResponse.Data.Status.ShouldBe("NaoCancelado");
    }
    [Fact]
    public async Task Post_CriarVenda_DeveValidarExternalIdentities()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        command.ClienteNome = "Cliente Teste External Identity";
        command.FilialNome = "Filial Teste External Identity";
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        getResponse.Data!.ClienteNome.ShouldBe("Cliente Teste External Identity");
        getResponse.Data.FilialNome.ShouldBe("Filial Teste External Identity");
    }
    [Fact]
    public async Task Post_CriarVenda_DeveCalcularValorTotalCorretamente()
    {
        var command = VendaTestHelper.CriarVendaCommandValida(quantidadeItens: 2, quantidadePorItem: 5, valorUnitario: 100m);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        getResponse.Data!.ValorTotal.ShouldBe(900m);
    }
    [Fact]
    public async Task Post_CriarVenda_DeveCalcularDescontosPorItem()
    {
        var command = VendaTestHelper.CriarVendaCommandValida(quantidadeItens: 1, quantidadePorItem: 5, valorUnitario: 100m);
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
    public async Task Post_CriarVenda_DeveValidarCamposObrigatorios()
    {
        var command = VendaTestHelper.CriarVendaCommandInvalida();
        var response = await _client.PostAsync("/api/venda", command);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
    [Fact]
    public async Task Post_CriarVenda_DeveCriarComStatusNaoCancelado()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        getResponse.Data!.Status.ShouldBe("NaoCancelado");
    }
    #endregion
    #region READ (GET /api/venda e GET /api/venda/{id})
    [Fact]
    public async Task Get_ListarVendas_DeveRetornarListaComVendas()
    {
        var command1 = VendaTestHelper.CriarVendaCommandValida();
        var command2 = VendaTestHelper.CriarVendaCommandValida();
        var createResponse1 = await _client.PostAsync<Guid>("/api/venda", command1);
        var createResponse2 = await _client.PostAsync<Guid>("/api/venda", command2);
        createResponse1.Success.ShouldBeTrue();
        createResponse2.Success.ShouldBeTrue();
        var response = await _client.GetListAsync<VendaDto>("/api/venda");
        response.Success.ShouldBeTrue();
        response.Data.ShouldNotBeNull();
        response.Data!.Count.ShouldBeGreaterThanOrEqualTo(2);
    }
    [Fact]
    public async Task Get_ObterVendaPorId_DeveRetornarVendaQuandoExiste()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var response = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        response.Success.ShouldBeTrue();
        response.Data.ShouldNotBeNull();
        response.Data!.Id.ShouldBe(vendaId);
    }
    [Fact]
    public async Task Get_ObterVendaPorId_DeveRetornar404QuandoNaoExiste()
    {
        var vendaIdInexistente = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/venda/{vendaIdInexistente}");
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
    [Fact]
    public async Task Get_ObterVendaPorId_DeveRetornarTodosOsCamposObrigatorios()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var response = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        response.Success.ShouldBeTrue();
        response.Data.ShouldNotBeNull();
        response.Data!.Id.ShouldNotBe(Guid.Empty);
        response.Data.NumeroVenda.ShouldNotBeNullOrEmpty();
        response.Data.Data.ShouldNotBe(default(DateTime));
        response.Data.ClienteId.ShouldNotBe(Guid.Empty);
        response.Data.ClienteNome.ShouldNotBeNullOrEmpty();
        response.Data.FilialId.ShouldNotBe(Guid.Empty);
        response.Data.FilialNome.ShouldNotBeNullOrEmpty();
        response.Data.Status.ShouldNotBeNullOrEmpty();
        response.Data.ValorTotal.ShouldBeGreaterThanOrEqualTo(0);
    }
    [Fact]
    public async Task Get_ObterVendaPorId_DeveRetornarItensComDescontosCalculados()
    {
        var command = VendaTestHelper.CriarVendaCommandValida(quantidadeItens: 1, quantidadePorItem: 5, valorUnitario: 100m);
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var response = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        response.Success.ShouldBeTrue();
        response.Data.ShouldNotBeNull();
        response.Data!.Itens.ShouldNotBeEmpty();
        var item = response.Data.Itens.First();
        item.Desconto.ShouldBeGreaterThan(0);
        item.ValorTotalItem.ShouldBeGreaterThan(0);
    }
    [Fact]
    public async Task Get_ObterVendaPorId_DeveRetornarStatusDaVenda()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var response = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        response.Success.ShouldBeTrue();
        response.Data.ShouldNotBeNull();
        response.Data!.Status.ShouldBeOneOf("NaoCancelado", "Cancelado");
    }
    #endregion
    #region UPDATE (PUT /api/venda/{id})
    [Fact]
    public async Task Put_AtualizarVenda_DeveRecalcularValorTotal()
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
                new()
                {
                    ItemId = itemId,
                    Quantidade = 10
                }
            }
        };
        await _client.PutAsync($"/api/venda/{vendaId}", updateCommand);
        var getResponseDepois = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponseDepois.Success.ShouldBeTrue();
        getResponseDepois.Data.ShouldNotBeNull();
        getResponseDepois.Data!.ValorTotal.ShouldBe(800m);
    }
    [Fact]
    public async Task Put_AtualizarVenda_DeveRetornar400QuandoNaoExiste()
    {
        var vendaIdInexistente = Guid.NewGuid();
        var updateCommand = new UpdateVendaCommand { Id = vendaIdInexistente };
        var response = await _client.PutAsync($"/api/venda/{vendaIdInexistente}", updateCommand);
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
    #endregion
    #region DELETE (DELETE /api/venda/{id})
    [Fact]
    public async Task Delete_CancelarVenda_DeveRetornar204NoContent()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var response = await _client.DeleteAsync($"/api/venda/{vendaId}");
        response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
    [Fact]
    public async Task Delete_CancelarVenda_DeveAlterarStatusParaCancelado()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        createResponse.Success.ShouldBeTrue();
        var vendaId = createResponse.Data;
        var deleteResponse = await _client.DeleteAsync($"/api/venda/{vendaId}");
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        var getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{vendaId}");
        getResponse.Success.ShouldBeTrue();
        getResponse.Data.ShouldNotBeNull();
        getResponse.Data!.Status.ShouldBe("Cancelado");
    }
    [Fact]
    public async Task Delete_CancelarVenda_DeveRetornar400QuandoNaoExiste()
    {
        var vendaIdInexistente = Guid.NewGuid();
        var response = await _client.DeleteAsync($"/api/venda/{vendaIdInexistente}");
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
    #endregion
}
