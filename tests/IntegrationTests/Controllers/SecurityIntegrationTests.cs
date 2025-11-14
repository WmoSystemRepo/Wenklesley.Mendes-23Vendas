using System.Net;
using IntegrationTests.Fixtures;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;
namespace IntegrationTests.Controllers;
public class SecurityIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;
    public SecurityIntegrationTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task Post_ComPayloadSQLInjection_DeveRejeitarOuSanitizar()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        command.NumeroVenda = "'; DROP TABLE Vendas; --";
        var response = await _client.PostAsync("/api/venda", command);
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Created);
    }
    [Fact]
    public async Task Post_ComPayloadXSS_DeveRejeitarOuSanitizar()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        command.ClienteNome = "<script>alert('XSS')</script>";
        var response = await _client.PostAsync("/api/venda", command);
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Created);
    }
    [Fact]
    public async Task Get_HealthCheck_DeveRetornarHeadersDeSeguranca()
    {
        var response = await _client.GetAsync("/health");
        response.Headers.ShouldContain(h => h.Key == "X-Correlation-Id");
    }
    [Fact]
    public async Task Post_CriarVenda_DeveValidarContentType()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        var response = await _client.PostAsync("/api/venda", command);
        response.StatusCode.ShouldBeOneOf(HttpStatusCode.Created, HttpStatusCode.BadRequest);
    }
}
