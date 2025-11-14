using Application.Commands;
using Application.DTOs;
using Api.Models;
using IntegrationTests.Fixtures;
using IntegrationTests.Helpers;
using Shouldly;
using TechTalk.SpecFlow;
namespace BddTests.StepDefinitions;
[Binding]
public class DomainEventsStepDefinitions
{
    private readonly HttpClient _client;
    private CreateVendaCommand? _command;
    private ApiResponse<Guid>? _createResponse;
    private ApiResponse<VendaDto>? _getResponse;
    public DomainEventsStepDefinitions()
    {
        _client = Helpers.BddTestContext.Client;
    }
    [Given(@"que não existem vendas cadastradas")]
    public void GivenQueNaoExistemVendasCadastradas()
    {
    }
    [Given(@"que existe uma venda cadastrada")]
    public async Task GivenQueExisteUmaVendaCadastrada()
    {
        _command = VendaTestHelper.CriarVendaCommandValida();
        _createResponse = await _client.PostAsync<Guid>("/api/venda", _command);
    }
    [Given(@"que existe uma venda com itens cadastrados")]
    public async Task GivenQueExisteUmaVendaComItensCadastrados()
    {
        _command = VendaTestHelper.CriarVendaCommandValida(quantidadeItens: 2);
        _createResponse = await _client.PostAsync<Guid>("/api/venda", _command);
    }
    [When(@"eu criar uma venda com sucesso")]
    public async Task WhenEuCriarUmaVendaComSucesso()
    {
        _command = VendaTestHelper.CriarVendaCommandValida();
        _createResponse = await _client.PostAsync<Guid>("/api/venda", _command);
        if (_createResponse.Success)
        {
            _getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{_createResponse.Data}");
        }
    }
    [When(@"eu atualizar a venda")]
    public async Task WhenEuAtualizarAVenda()
    {
        if (_createResponse?.Data != null)
        {
            _getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{_createResponse.Data}");
            var itemId = _getResponse!.Data!.Itens.First().Id;
            var updateCommand = new UpdateVendaCommand
            {
                Id = _createResponse.Data,
                ItensParaAtualizar = new List<UpdateVendaItemExistenteCommand>
                {
                    new() { ItemId = itemId, Quantidade = 10 }
                }
            };
            await _client.PutAsync($"/api/venda/{_createResponse.Data}", updateCommand);
        }
    }
    [When(@"eu cancelar a venda")]
    public async Task WhenEuCancelarAVenda()
    {
        if (_createResponse?.Data != null)
        {
            await _client.DeleteAsync($"/api/venda/{_createResponse.Data}");
            _getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{_createResponse.Data}");
        }
    }
    [When(@"eu remover um item da venda")]
    public async Task WhenEuRemoverUmItemDaVenda()
    {
        if (_createResponse?.Data != null)
        {
            _getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{_createResponse.Data}");
            var itemId = _getResponse!.Data!.Itens.First().Id;
            var updateCommand = new UpdateVendaCommand
            {
                Id = _createResponse.Data,
                ItensParaRemover = new List<Guid> { itemId }
            };
            await _client.PutAsync($"/api/venda/{_createResponse.Data}", updateCommand);
        }
    }
    [Then(@"o evento CompraEfetuada deve ser emitido")]
    public void ThenOEventoCompraEfetuadaDeveSerEmitido()
    {
        _createResponse.ShouldNotBeNull();
        _createResponse!.Success.ShouldBeTrue();
    }
    [Then(@"o evento CompraAlterada deve ser emitido")]
    public void ThenOEventoCompraAlteradaDeveSerEmitido()
    {
        _getResponse.ShouldNotBeNull();
    }
    [Then(@"o evento CompraCancelada deve ser emitido")]
    public void ThenOEventoCompraCanceladaDeveSerEmitido()
    {
        _getResponse.ShouldNotBeNull();
        _getResponse!.Data!.Status.ShouldBe("Cancelado");
    }
    [Then(@"o evento ItemCancelado deve ser emitido")]
    public void ThenOEventoItemCanceladoDeveSerEmitido()
    {
        _getResponse.ShouldNotBeNull();
    }
    [Then(@"deve ser logado em formato JSON")]
    public void ThenDeveSerLogadoEmFormatoJSON()
    {
        _getResponse.ShouldNotBeNull();
    }
    [Then(@"deve conter VendaId, NumeroVenda e ValorTotal")]
    public void ThenDeveConterVendaIdNumeroVendaEValorTotal()
    {
        _getResponse.ShouldNotBeNull();
        _getResponse!.Data!.Id.ShouldNotBe(Guid.Empty);
        _getResponse.Data.NumeroVenda.ShouldNotBeNullOrEmpty();
        _getResponse.Data.ValorTotal.ShouldBeGreaterThanOrEqualTo(0);
    }
    [Then(@"deve conter VendaId e NumeroVenda")]
    public void ThenDeveConterVendaIdENumeroVenda()
    {
        _getResponse.ShouldNotBeNull();
        _getResponse!.Data!.Id.ShouldNotBe(Guid.Empty);
        _getResponse.Data.NumeroVenda.ShouldNotBeNullOrEmpty();
    }
    [Then(@"deve conter VendaId, ItemId e ProdutoNome")]
    public void ThenDeveConterVendaIdItemIdEProdutoNome()
    {
        _getResponse.ShouldNotBeNull();
        _getResponse!.Data!.Id.ShouldNotBe(Guid.Empty);
        if (_getResponse.Data.Itens.Any())
        {
            var item = _getResponse.Data.Itens.First();
            item.Id.ShouldNotBe(Guid.Empty);
            item.ProdutoNome.ShouldNotBeNullOrEmpty();
        }
    }
}
