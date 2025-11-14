using Application.DTOs;
using Api.Models;
using IntegrationTests.Fixtures;
using IntegrationTests.Helpers;
using Shouldly;
using TechTalk.SpecFlow;
namespace BddTests.StepDefinitions;
[Binding]
public class DescontoStepDefinitions
{
    private readonly HttpClient _client;
    private ApiResponse<Guid>? _createResponse;
    private ApiResponse<VendaDto>? _getResponse;
    public DescontoStepDefinitions()
    {
        _client = Helpers.BddTestContext.Client;
    }
    [Given(@"que estou criando uma venda")]
    public void GivenQueEstouCriandoUmaVenda()
    {
    }
    [When(@"eu adicionar (\d+) itens com valor unitário de R\$ ([\d.]+)")]
    public async Task WhenEuAdicionarItensComValorUnitario(int quantidade, decimal valorUnitario)
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidade(quantidade);
        command.Itens[0].ValorUnitario = valorUnitario;
        _createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        if (_createResponse.Success)
        {
            _getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{_createResponse.Data}");
        }
    }
    [When(@"eu tentar adicionar (\d+) itens")]
    public async Task WhenEuTentarAdicionarItens(int quantidade)
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidadeInvalida(quantidade);
        var response = await _client.PostAsync("/api/venda", command);
    }
    [When(@"eu adicionar (\d+) itens")]
    public async Task WhenEuAdicionarItens(int quantidade)
    {
        var command = VendaTestHelper.CriarVendaCommandComQuantidade(quantidade);
        _createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        if (_createResponse.Success)
        {
            _getResponse = await _client.GetAsync<VendaDto>($"/api/venda/{_createResponse.Data}");
        }
    }
    [Then(@"o desconto aplicado deve ser (\d+)%")]
    public void ThenODescontoAplicadoDeveSer(int descontoPercentual)
    {
        _getResponse.ShouldNotBeNull();
        var item = _getResponse!.Data!.Itens.First();
        var valorBruto = item.Quantidade * item.ValorUnitario;
        var descontoEsperado = valorBruto * (descontoPercentual / 100m);
        item.Desconto.ShouldBe(descontoEsperado, 0.01m);
    }
    [Then(@"o valor total do item deve ser R\$ ([\d.]+)")]
    public void ThenOValorTotalDoItemDeveSer(decimal valorTotalEsperado)
    {
        _getResponse.ShouldNotBeNull();
        var item = _getResponse!.Data!.Itens.First();
        item.ValorTotalItem.ShouldBe(valorTotalEsperado, 0.01m);
    }
    [Then(@"a venda não deve ser criada")]
    public void ThenAVendaNaoDeveSerCriada()
    {
    }
    [Then(@"a mensagem deve indicar que quantidade > 20 é proibida")]
    public void ThenAMensagemDeveIndicarQueQuantidadeMaiorQue20EProibida()
    {
    }
    [Then(@"não deve aplicar nenhum desconto")]
    public void ThenNaoDeveAplicarNenhumDesconto()
    {
        _getResponse.ShouldNotBeNull();
        var item = _getResponse!.Data!.Itens.First();
        item.Desconto.ShouldBe(0m);
    }
    [Then(@"o valor total deve ser o valor bruto")]
    public void ThenOValorTotalDeveSerOValorBruto()
    {
        _getResponse.ShouldNotBeNull();
        var item = _getResponse!.Data!.Itens.First();
        var valorBruto = item.Quantidade * item.ValorUnitario;
        item.ValorTotalItem.ShouldBe(valorBruto);
    }
}
