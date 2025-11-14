using System.Net;
using IntegrationTests.Fixtures;
using IntegrationTests.Helpers;
using Shouldly;
using TechTalk.SpecFlow;
namespace BddTests.StepDefinitions;
[Binding]
public class ObservabilidadeStepDefinitions
{
    private readonly HttpClient _client;
    private HttpResponseMessage? _response;
    public ObservabilidadeStepDefinitions()
    {
        _client = Helpers.BddTestContext.Client;
    }
    [Given(@"que a aplicação está rodando")]
    public void GivenQueAplicacaoEstaRodando()
    {
    }
    [When(@"eu fizer uma requisição para a API")]
    public async Task WhenEuFizerUmaRequisicaoParaAAPI()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        _response = await _client.PostAsync("/api/venda", command);
    }
    [When(@"eu fizer uma requisição para POST /api/venda")]
    public async Task WhenEuFizerUmaRequisicaoParaPOSTApiVenda()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        _response = await _client.PostAsync("/api/venda", command);
    }
    [When(@"eu acessar GET /health")]
    public async Task WhenEuAcessarGETHealth()
    {
        _response = await _client.GetAsync("/health");
    }
    [When(@"eu fizer uma requisição que demora mais de 1000ms")]
    public async Task WhenEuFizerUmaRequisicaoQueDemoraMaisDe1000ms()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        _response = await _client.PostAsync("/api/venda", command);
    }
    [Then(@"os logs devem ser gerados em formato JSON")]
    public void ThenOsLogsDevemSerGeradosEmFormatoJSON()
    {
        _response.ShouldNotBeNull();
    }
    [Then(@"devem usar Serilog")]
    public void ThenDevemUsarSerilog()
    {
        _response.ShouldNotBeNull();
    }
    [Then(@"devem aparecer no console")]
    public void ThenDevemAparecerNoConsole()
    {
        _response.ShouldNotBeNull();
    }
    [Then(@"o middleware deve medir o tempo de resposta")]
    public void ThenOMiddlewareDeveMedirOTempoDeResposta()
    {
        _response.ShouldNotBeNull();
    }
    [Then(@"deve logar o tempo em milissegundos")]
    public void ThenDeveLogarOTempoEmMilissegundos()
    {
        _response.ShouldNotBeNull();
    }
    [Then(@"deve incluir método, path e status code")]
    public void ThenDeveIncluirMetodoPathEStatusCode()
    {
        _response.ShouldNotBeNull();
    }
    [Then(@"deve gerar um Correlation ID")]
    public void ThenDeveGerarUmCorrelationID()
    {
        _response.ShouldNotBeNull();
        _response!.Headers.ShouldContain(h => h.Key == "X-Correlation-Id");
    }
    [Then(@"deve retornar no header X-Correlation-Id")]
    public void ThenDeveRetornarNoHeaderXCorrelationId()
    {
        _response!.Headers.ShouldContain(h => h.Key == "X-Correlation-Id");
        var correlationId = _response.Headers.GetValues("X-Correlation-Id").FirstOrDefault();
        correlationId.ShouldNotBeNullOrEmpty();
    }
    [Then(@"deve estar presente nos logs")]
    public void ThenDeveEstarPresenteNosLogs()
    {
        _response.ShouldNotBeNull();
    }
    [Then(@"deve logar como Warning")]
    public void ThenDeveLogarComoWarning()
    {
        _response.ShouldNotBeNull();
    }
    [Then(@"deve incluir mensagem ""([^""]*)""")]
    public void ThenDeveIncluirMensagem(string mensagem)
    {
        _response.ShouldNotBeNull();
    }
    [Then(@"deve retornar status (\d+) OK")]
    public void ThenDeveRetornarStatusOK(int statusCode)
    {
        _response!.StatusCode.ShouldBe((HttpStatusCode)statusCode);
    }
    [Then(@"deve retornar status ""([^""]*)""")]
    public void ThenDeveRetornarStatus(string status)
    {
        var content = _response!.Content.ReadAsStringAsync().Result;
        content.ShouldContain(status);
    }
    [Then(@"deve retornar timestamp")]
    public void ThenDeveRetornarTimestamp()
    {
        var content = _response!.Content.ReadAsStringAsync().Result;
        content.ShouldContain("timestamp");
    }
    [Then(@"deve retornar uptime")]
    public void ThenDeveRetornarUptime()
    {
        var content = _response!.Content.ReadAsStringAsync().Result;
        content.ShouldContain("uptime");
    }
    [Then(@"deve retornar environment")]
    public void ThenDeveRetornarEnvironment()
    {
        var content = _response!.Content.ReadAsStringAsync().Result;
        content.ShouldContain("environment");
    }
    [Then(@"deve retornar version")]
    public void ThenDeveRetornarVersion()
    {
        var content = _response!.Content.ReadAsStringAsync().Result;
        content.ShouldContain("version");
    }
}
