using System.Net;
using IntegrationTests.Fixtures;
using IntegrationTests.Helpers;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;
namespace IntegrationTests.Controllers;
public class ObservabilidadeIntegrationTests : IClassFixture<ApiWebApplicationFactory>
{
    private readonly HttpClient _client;
    public ObservabilidadeIntegrationTests(ApiWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }
    [Fact]
    public async Task Get_HealthCheck_DeveRetornar200OK()
    {
        var response = await _client.GetAsync("/health");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
    [Fact]
    public async Task Get_HealthCheck_DeveRetornarStatusHealthy()
    {
        var response = await _client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("healthy");
    }
    [Fact]
    public async Task Get_HealthCheck_DeveRetornarTimestamp()
    {
        var response = await _client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("timestamp");
    }
    [Fact]
    public async Task Get_HealthCheck_DeveRetornarUptime()
    {
        var response = await _client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("uptime");
    }
    [Fact]
    public async Task Get_HealthCheck_DeveRetornarEnvironment()
    {
        var response = await _client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("environment");
    }
    [Fact]
    public async Task Get_HealthCheck_DeveRetornarVersion()
    {
        var response = await _client.GetAsync("/health");
        var content = await response.Content.ReadAsStringAsync();
        content.ShouldContain("version");
    }
    [Fact]
    public async Task Post_CriarVenda_DeveRetornarCorrelationIdNoHeader()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        var response = await _client.PostAsync("/api/venda", command);
        response.Headers.ShouldContain(h => h.Key == "X-Correlation-Id");
        var correlationId = response.Headers.GetValues("X-Correlation-Id").FirstOrDefault();
        correlationId.ShouldNotBeNullOrEmpty();
    }
    [Fact]
    public async Task Get_ObterVenda_DeveRetornarCorrelationIdNoHeader()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        var createResponse = await _client.PostAsync<Guid>("/api/venda", command);
        var vendaId = createResponse.Data;
        var response = await _client.GetAsync($"/api/venda/{vendaId}");
        response.Headers.ShouldContain(h => h.Key == "X-Correlation-Id");
        var correlationId = response.Headers.GetValues("X-Correlation-Id").FirstOrDefault();
        correlationId.ShouldNotBeNullOrEmpty();
    }
    [Fact]
    public async Task Post_CriarVenda_DeveIncluirCorrelationIdNaResposta()
    {
        var command = VendaTestHelper.CriarVendaCommandValida();
        var response = await _client.PostAsync<Guid>("/api/venda", command);
        response.CorrelationId.ShouldNotBeNullOrEmpty();
    }
    [Fact]
    public async Task Get_ListarVendas_DeveRetornarCorrelationId()
    {
        var response = await _client.GetListAsync<Application.DTOs.VendaDto>("/api/venda");
        response.CorrelationId.ShouldNotBeNullOrEmpty();
    }
}
