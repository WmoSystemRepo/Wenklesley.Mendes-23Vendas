using Application.Commands;
using Application.DTOs;
using Api.Models;
using IntegrationTests.Fixtures;
using IntegrationTests.Helpers;
using Shouldly;
using TechTalk.SpecFlow;
namespace BddTests.StepDefinitions;
[Binding]
public class VendaStepDefinitions
{
    private readonly HttpClient _client;
    private CreateVendaCommand? _command;
    private ApiResponse<Guid>? _createResponse;
    private ApiResponse<VendaDto>? _getResponse;
    private HttpResponseMessage? _httpResponse;
    public VendaStepDefinitions()
    {
        _client = Helpers.BddTestContext.Client;
    }
    [Given(@"que não existem vendas cadastradas")]
    public void GivenQueNaoExistemVendasCadastradas()
    {
    }
    [Given(@"que estou criando uma venda")]
    public void GivenQueEstouCriandoUmaVenda()
    {
        _command = VendaTestHelper.CriarVendaCommandValida();
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
    [When(@"eu criar uma venda com os seguintes dados:")]
    public async Task WhenEuCriarUmaVendaComOsSeguintesDados(Table table)
    {
        var row = table.Rows[0];
        _command = new CreateVendaCommand
        {
            NumeroVenda = row["NumeroVenda"],
            ClienteNome = row["ClienteNome"],
            FilialNome = row["FilialNome"],
            ClienteId = Guid.NewGuid(),
            FilialId = Guid.NewGuid(),
            Itens = new List<CreateVendaItemCommand>
            {
                new()
                {
                    ProdutoId = Guid.NewGuid(),
                    ProdutoNome = "Produto Teste",
                    Quantidade = int.Parse(row["Quantidade"]),
                    ValorUnitario = decimal.Parse(row["ValorUnitario"])
                }
            }
        };
        _createResponse = await _client.PostAsync<Guid>("/api/venda", _command);
    }
    [When(@"eu criar uma venda com sucesso")]
    public async Task WhenEuCriarUmaVendaComSucesso()
    {
        _command = VendaTestHelper.CriarVendaCommandValida();
        _createResponse = await _client.PostAsync<Guid>("/api/venda", _command);
    }
    [When(@"eu tentar criar uma venda com 21 itens")]
    public async Task WhenEuTentarCriarUmaVendaCom21Itens()
    {
        _command = VendaTestHelper.CriarVendaCommandComQuantidadeInvalida(21);
        _httpResponse = await _client.PostAsync("/api/venda", _command);
    }
    [When(@"eu adicionar (\d+) itens com valor unitário de R\$ ([\d.]+)")]
    public async Task WhenEuAdicionarItensComValorUnitario(int quantidade, decimal valorUnitario)
    {
        _command = VendaTestHelper.CriarVendaCommandComQuantidade(quantidade);
        _command.Itens[0].ValorUnitario = valorUnitario;
        _createResponse = await _client.PostAsync<Guid>("/api/venda", _command);
        if (_createResponse.Success)
        {
            _getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{_createResponse.Data}");
        }
    }
    [When(@"eu tentar adicionar (\d+) itens")]
    public async Task WhenEuTentarAdicionarItens(int quantidade)
    {
        _command = VendaTestHelper.CriarVendaCommandComQuantidadeInvalida(quantidade);
        _httpResponse = await _client.PostAsync("/api/venda", _command);
    }
    [When(@"eu adicionar (\d+) itens")]
    public async Task WhenEuAdicionarItens(int quantidade)
    {
        _command = VendaTestHelper.CriarVendaCommandComQuantidade(quantidade);
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
            var updateCommand = new UpdateVendaCommand
            {
                Id = _createResponse.Data,
                ItensParaAtualizar = new List<UpdateVendaItemExistenteCommand>
                {
                    new() { ItemId = Guid.NewGuid(), Quantidade = 10 }
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
    [When(@"eu criar uma venda com todos os campos preenchidos")]
    public async Task WhenEuCriarUmaVendaComTodosOsCamposPreenchidos()
    {
        _command = VendaTestHelper.CriarVendaCommandValida();
        _createResponse = await _client.PostAsync<Guid>("/api/venda", _command);
        if (_createResponse.Success)
        {
            _getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{_createResponse.Data}");
        }
    }
    [Then(@"a venda deve ser criada com sucesso")]
    public void ThenAVendaDeveSerCriadaComSucesso()
    {
        _createResponse.ShouldNotBeNull();
        _createResponse!.Success.ShouldBeTrue();
        _createResponse.Data.ShouldNotBe(Guid.Empty);
    }
    [Then(@"o valor total deve ser calculado corretamente")]
    public void ThenOValorTotalDeveSerCalculadoCorretamente()
    {
        _getResponse.ShouldNotBeNull();
        _getResponse!.Data!.ValorTotal.ShouldBeGreaterThan(0);
    }
    [Then(@"deve aplicar desconto de (\d+)% para (\d+) itens")]
    public void ThenDeveAplicarDescontoDePorcentoParaItens(int descontoPercentual, int quantidade)
    {
        _getResponse.ShouldNotBeNull();
        var item = _getResponse!.Data!.Itens.First();
        var valorEsperado = quantidade * 100m * (1 - descontoPercentual / 100m);
        item.ValorTotalItem.ShouldBe(valorEsperado, 0.01m);
    }
    [Then(@"a venda não deve ser criada")]
    public void ThenAVendaNaoDeveSerCriada()
    {
        _httpResponse?.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }
    [Then(@"deve retornar erro de validação")]
    public void ThenDeveRetornarErroDeValidacao()
    {
        _httpResponse?.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
    }
    [Then(@"a venda deve conter número da venda")]
    public void ThenAVendaDeveConterNumeroDaVenda()
    {
        _getResponse!.Data!.NumeroVenda.ShouldNotBeNullOrEmpty();
    }
    [Then(@"deve conter data da venda")]
    public void ThenDeveConterDataDaVenda()
    {
        _getResponse!.Data!.Data.ShouldNotBe(default(DateTime));
    }
    [Then(@"deve conter cliente \(ID e nome\)")]
    public void ThenDeveConterClienteIDENome()
    {
        _getResponse!.Data!.ClienteId.ShouldNotBe(Guid.Empty);
        _getResponse.Data.ClienteNome.ShouldNotBeNullOrEmpty();
    }
    [Then(@"deve conter filial \(ID e nome\)")]
    public void ThenDeveConterFilialIDENome()
    {
        _getResponse!.Data!.FilialId.ShouldNotBe(Guid.Empty);
        _getResponse.Data.FilialNome.ShouldNotBeNullOrEmpty();
    }
    [Then(@"deve conter valor total")]
    public void ThenDeveConterValorTotal()
    {
        _getResponse!.Data!.ValorTotal.ShouldBeGreaterThanOrEqualTo(0);
    }
    [Then(@"deve conter status NaoCancelado")]
    public void ThenDeveConterStatusNaoCancelado()
    {
        _getResponse!.Data!.Status.ShouldBe("NaoCancelado");
    }
}
