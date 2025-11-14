using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Produces("application/json")]
public class DashboardController : ControllerBase
{
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(ILogger<DashboardController> logger)
    {
        _logger = logger;
    }

    [HttpGet("tests")]
    public IActionResult GetTests()
    {
        var tests = new object[]
        {
            new { id = "1", name = "CriarVenda_DeveCriarVendaComSucesso", type = "unit", status = "passed", duration = 15, timestamp = DateTime.UtcNow, message = (string?)null },
            new { id = "2", name = "Post_CriarVenda_DeveRetornar201CreatedComId", type = "integration", status = "passed", duration = 120, timestamp = DateTime.UtcNow, message = (string?)null },
            new { id = "3", name = "Criar venda com sucesso", type = "bdd", status = "passed", duration = 200, timestamp = DateTime.UtcNow, message = (string?)null },
            new { id = "4", name = "Validar_NumeroVendaVazio_DeveFalhar", type = "unit", status = "failed", duration = 5, timestamp = DateTime.UtcNow, message = "Validação falhou: Número de venda é obrigatório" }
        };

        return Ok(tests);
    }

    [HttpGet("tests/stats")]
    public IActionResult GetTestStats()
    {
        var stats = new
        {
            total = 119,
            passed = 118,
            failed = 1,
            running = 0,
            pending = 0,
            successRate = 99
        };

        return Ok(stats);
    }

    [HttpPost("tests/run")]
    public IActionResult RunTests()
    {
        _logger.LogInformation("Executando testes via dashboard");
        return Ok(new { message = "Testes iniciados" });
    }

    [HttpDelete("tests")]
    public IActionResult ClearTests()
    {
        _logger.LogInformation("Testes limpos via dashboard");
        return NoContent();
    }

    [HttpGet("tests/{testName}/scenarios")]
    public IActionResult GetTestScenarios([FromRoute] string testName)
    {
        var decodedTestName = Uri.UnescapeDataString(testName);
        
        var scenarios = GetTestScenariosFromDocumentation(decodedTestName);
        
        if (scenarios == null)
        {
            return Ok(new
            {
                testName = decodedTestName,
                type = "unknown",
                description = "Cenários não disponíveis para este teste",
                scenarios = Array.Empty<object>(),
                file = "N/A"
            });
        }

        return Ok(scenarios);
    }

    private object? GetTestScenariosFromDocumentation(string testName)
    {
        var testScenariosMap = new Dictionary<string, object>
        {
            ["CriarVenda_DeveCriarVendaComStatusNaoCancelado"] = new
            {
                testName = "CriarVenda_DeveCriarVendaComStatusNaoCancelado",
                type = "unit",
                description = "Verifica criação de venda com status inicial correto",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Criação de venda", expectedResult = "Venda criada com status NaoCancelado" }
                },
                file = "tests/UnitTests/Domain/Entities/VendaTests.cs"
            },
            ["AdicionarItem_DeveAdicionarItemEVincularAVenda"] = new
            {
                testName = "AdicionarItem_DeveAdicionarItemEVincularAVenda",
                type = "unit",
                description = "Verifica adição de item à venda",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Adicionar item à venda", expectedResult = "Item adicionado e vinculado à venda" }
                },
                file = "tests/UnitTests/Domain/Entities/VendaTests.cs"
            },
            ["AdicionarItem_DeveRecalcularValorTotal"] = new
            {
                testName = "AdicionarItem_DeveRecalcularValorTotal",
                type = "unit",
                description = "Verifica recálculo automático do valor total",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Adicionar item", expectedResult = "Valor total recalculado automaticamente" }
                },
                file = "tests/UnitTests/Domain/Entities/VendaTests.cs"
            },
            ["AdicionarItem_DeveEmitirEventoCompraEfetuada"] = new
            {
                testName = "AdicionarItem_DeveEmitirEventoCompraEfetuada",
                type = "unit",
                description = "Verifica emissão de evento de domínio",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Adicionar item", expectedResult = "Evento CompraEfetuada emitido" }
                },
                file = "tests/UnitTests/Domain/Entities/VendaTests.cs"
            },
            ["RemoverItem_DeveRemoverItemEVincularAVenda"] = new
            {
                testName = "RemoverItem_DeveRemoverItemEVincularAVenda",
                type = "unit",
                description = "Verifica remoção de item da venda",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Remover item da venda", expectedResult = "Item removido da venda" }
                },
                file = "tests/UnitTests/Domain/Entities/VendaTests.cs"
            },
            ["Cancelar_DeveAlterarStatusParaCancelado"] = new
            {
                testName = "Cancelar_DeveAlterarStatusParaCancelado",
                type = "unit",
                description = "Verifica cancelamento de venda",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Cancelar venda", expectedResult = "Status alterado para Cancelado" }
                },
                file = "tests/UnitTests/Domain/Entities/VendaTests.cs"
            },
            ["CriarItem_QuantidadeEntre5E9_DeveAplicar10PorcentoDesconto"] = new
            {
                testName = "CriarItem_QuantidadeEntre5E9_DeveAplicar10PorcentoDesconto",
                type = "unit",
                description = "Verifica desconto de 10% para 5-9 itens",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Criar item com quantidade entre 5 e 9", expectedResult = "Desconto de 10% aplicado" }
                },
                file = "tests/UnitTests/Domain/Entities/VendaItemTests.cs"
            },
            ["CriarItem_QuantidadeEntre10E20_DeveAplicar20PorcentoDesconto"] = new
            {
                testName = "CriarItem_QuantidadeEntre10E20_DeveAplicar20PorcentoDesconto",
                type = "unit",
                description = "Verifica desconto de 20% para 10-20 itens",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Criar item com quantidade entre 10 e 20", expectedResult = "Desconto de 20% aplicado" }
                },
                file = "tests/UnitTests/Domain/Entities/VendaItemTests.cs"
            },
            ["Handle_DeveCriarVendaComSucesso"] = new
            {
                testName = "Handle_DeveCriarVendaComSucesso",
                type = "unit",
                description = "Verifica criação de venda através do handler",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Criar venda via handler", expectedResult = "Venda criada com sucesso" }
                },
                file = "tests/UnitTests/Application/Handlers/CreateVendaHandlerTests.cs"
            },
            ["CriarVenda_DeveCriarVendaComSucesso"] = new
            {
                testName = "CriarVenda_DeveCriarVendaComSucesso",
                type = "unit",
                description = "Verifica criação de venda com sucesso",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Criar venda com dados válidos", expectedResult = "Venda criada com sucesso e ID retornado" },
                    new { id = "scenario-2", description = "Validar que o repositório foi chamado", expectedResult = "Método AddAsync foi invocado" },
                    new { id = "scenario-3", description = "Validar que as mudanças foram salvas", expectedResult = "SaveChangesAsync foi chamado e retornou sucesso" }
                },
                file = "tests/UnitTests/Application/Handlers/CreateVendaHandlerTests.cs"
            },
            ["Validar_NumeroVendaVazio_DeveFalhar"] = new
            {
                testName = "Validar_NumeroVendaVazio_DeveFalhar",
                type = "unit",
                description = "Verifica validação de número de venda obrigatório",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Tentar criar venda sem número", expectedResult = "Validação deve falhar com erro" },
                    new { id = "scenario-2", description = "Verificar mensagem de erro retornada", expectedResult = "Mensagem de erro deve indicar campo obrigatório" }
                },
                file = "tests/UnitTests/Application/Validators/CreateVendaCommandValidatorTests.cs"
            },
            ["Post_CriarVenda_DeveRetornar201CreatedComId"] = new
            {
                testName = "Post_CriarVenda_DeveRetornar201CreatedComId",
                type = "integration",
                description = "Verifica criação com retorno de ID",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Criar venda via API POST", expectedResult = "Status 201 Created com ID retornado" }
                },
                file = "tests/IntegrationTests/Controllers/VendaControllerIntegrationTests.cs"
            },
            ["Post_CriarVenda_DeveCalcularDescontosPorItem"] = new
            {
                testName = "Post_CriarVenda_DeveCalcularDescontosPorItem",
                type = "integration",
                description = "Verifica cálculo de descontos",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Criar venda com itens que recebem desconto", expectedResult = "Descontos calculados corretamente por item" }
                },
                file = "tests/IntegrationTests/Controllers/VendaControllerIntegrationTests.cs"
            },
            ["Get_ObterVendaPorId_DeveRetornarVendaExistente"] = new
            {
                testName = "Get_ObterVendaPorId_DeveRetornarVendaExistente",
                type = "integration",
                description = "Verifica obtenção por ID",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Obter venda existente via GET", expectedResult = "Venda retornada com sucesso" }
                },
                file = "tests/IntegrationTests/Controllers/VendaControllerIntegrationTests.cs"
            },
            ["Put_AtualizarVenda_DeveAtualizarItens"] = new
            {
                testName = "Put_AtualizarVenda_DeveAtualizarItens",
                type = "integration",
                description = "Verifica atualização de itens",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Atualizar venda via PUT", expectedResult = "Itens atualizados com sucesso" }
                },
                file = "tests/IntegrationTests/Controllers/VendaControllerIntegrationTests.cs"
            },
            ["Delete_CancelarVenda_DeveAlterarStatusParaCancelado"] = new
            {
                testName = "Delete_CancelarVenda_DeveAlterarStatusParaCancelado",
                type = "integration",
                description = "Verifica cancelamento",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Cancelar venda via DELETE", expectedResult = "Status alterado para Cancelado" }
                },
                file = "tests/IntegrationTests/Controllers/VendaControllerIntegrationTests.cs"
            },
            ["Post_Com5Itens_DeveAplicar10PorcentoDesconto"] = new
            {
                testName = "Post_Com5Itens_DeveAplicar10PorcentoDesconto",
                type = "integration",
                description = "Verifica desconto de 10% para 5 itens",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Criar venda com 5 itens", expectedResult = "Desconto de 10% aplicado" }
                },
                file = "tests/IntegrationTests/Controllers/RegrasDeNegocioIntegrationTests.cs"
            },
            ["Post_Com10Itens_DeveAplicar20PorcentoDesconto"] = new
            {
                testName = "Post_Com10Itens_DeveAplicar20PorcentoDesconto",
                type = "integration",
                description = "Verifica desconto de 20% para 10 itens",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Criar venda com 10 itens", expectedResult = "Desconto de 20% aplicado" }
                },
                file = "tests/IntegrationTests/Controllers/RegrasDeNegocioIntegrationTests.cs"
            },
            ["Criar venda com sucesso"] = new
            {
                testName = "Criar venda com sucesso",
                type = "bdd",
                description = "Cria venda com dados válidos e verifica sucesso",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Criar venda com dados válidos", expectedResult = "Venda criada com sucesso" }
                },
                file = "tests/BddTests/Features/CriarVenda.feature"
            },
            ["Criar venda com quantidade acima do permitido"] = new
            {
                testName = "Criar venda com quantidade acima do permitido",
                type = "bdd",
                description = "Tenta criar venda com 21 itens e verifica erro",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Tentar criar venda com 21 itens", expectedResult = "Erro retornado (quantidade > 20 não permitida)" }
                },
                file = "tests/BddTests/Features/CriarVenda.feature"
            },
            ["Aplicar desconto baseado na quantidade"] = new
            {
                testName = "Aplicar desconto baseado na quantidade",
                type = "bdd",
                description = "Testa descontos para quantidades de 1 a 20 itens",
                scenarios = new[]
                {
                    new { id = "scenario-1", description = "Testar descontos para diferentes quantidades", expectedResult = "Descontos aplicados corretamente: 0% (1-4), 10% (5-9), 20% (10-20)" }
                },
                file = "tests/BddTests/Features/RegrasDeDesconto.feature"
            }
        };

        var normalizedName = testName.Trim();
        if (testScenariosMap.ContainsKey(normalizedName))
        {
            return testScenariosMap[normalizedName];
        }

        var found = testScenariosMap.FirstOrDefault(kvp => 
            string.Equals(kvp.Key, normalizedName, StringComparison.OrdinalIgnoreCase));
        
        if (found.Key != null)
        {
            return found.Value;
        }

        return null;
    }

    [HttpGet("logs")]
    public IActionResult GetLogs([FromQuery] string? level = null, [FromQuery] int limit = 100)
    {
        var logs = new List<object>
        {
            new
            {
                timestamp = DateTime.UtcNow.ToString("O"),
                level = "Information",
                message = "Evento de Domínio: CompraEfetuada",
                properties = new { VendaId = Guid.NewGuid(), NumeroVenda = "V001", ValorTotal = 450.00 }
            },
            new
            {
                timestamp = DateTime.UtcNow.AddSeconds(-1).ToString("O"),
                level = "Information",
                message = "Request processada com sucesso",
                properties = new { Method = "POST", Path = "/api/venda", Duration = 45, StatusCode = 201 }
            }
        };

        var filtered = logs.AsEnumerable();
        if (!string.IsNullOrEmpty(level) && level != "all")
        {
            filtered = logs.Where(l => ((dynamic)l).level == level);
        }

        return Ok(filtered.Take(limit));
    }

    [HttpDelete("logs")]
    public IActionResult ClearLogs()
    {
        _logger.LogInformation("Logs limpos via dashboard");
        return NoContent();
    }

    [HttpGet("git-info")]
    public IActionResult GetGitInfo()
    {
        var gitInfo = new
        {
            branch = "main",
            commit = "abc123",
            author = "Developer",
            message = "feat: adicionar dashboard Angular",
            timestamp = DateTime.UtcNow
        };

        return Ok(gitInfo);
    }

    [HttpGet("git/branches")]
    public IActionResult GetBranches()
    {
        var branches = new[]
        {
            new { name = "main", type = "main", description = "Branch principal de produção", example = "main" },
            new { name = "develop", type = "develop", description = "Branch de desenvolvimento", example = "develop" },
            new { name = "feature/nova-funcionalidade", type = "feature", description = "Branch para novas funcionalidades", example = "feature/adicionar-filtro-vendas" },
            new { name = "bugfix/corrigir-bug", type = "bugfix", description = "Branch para correção de bugs", example = "bugfix/corrigir-calculo-desconto" },
            new { name = "hotfix/correcao-urgente", type = "hotfix", description = "Branch para correções urgentes", example = "hotfix/corrigir-vulnerabilidade" },
            new { name = "release/v1.0.0", type = "release", description = "Branch para releases", example = "release/v1.0.0" }
        };

        return Ok(branches);
    }

    [HttpGet("git/flow-steps")]
    public IActionResult GetGitFlowSteps()
    {
        var steps = new[]
        {
            new { step = 1, title = "Criar feature branch", description = "git checkout -b feature/nova-funcionalidade develop", command = "git checkout -b feature/nova-funcionalidade develop", branch = "feature" },
            new { step = 2, title = "Desenvolver e commitar", description = "Fazer commits semânticos durante o desenvolvimento", command = "git commit -m 'feat: adicionar nova funcionalidade'", branch = "feature" },
            new { step = 3, title = "Merge para develop", description = "git checkout develop && git merge feature/nova-funcionalidade", command = "git checkout develop && git merge feature/nova-funcionalidade", branch = "develop" },
            new { step = 4, title = "Criar release", description = "git checkout -b release/v1.0.0 develop", command = "git checkout -b release/v1.0.0 develop", branch = "release" },
            new { step = 5, title = "Merge para main", description = "git checkout main && git merge release/v1.0.0", command = "git checkout main && git merge release/v1.0.0", branch = "main" }
        };

        return Ok(steps);
    }

    [HttpPost("git/validate-commit")]
    public IActionResult ValidateCommit([FromBody] CommitValidationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { isValid = false, message = "Mensagem de commit não pode ser vazia" });
        }

        var pattern = @"^(feat|fix|docs|style|refactor|test|chore)(\(.+\))?:\s.+";
        var match = System.Text.RegularExpressions.Regex.Match(request.Message, pattern);

        if (!match.Success)
        {
            return Ok(new
            {
                hash = "abc123",
                type = "invalid",
                scope = (string?)null,
                subject = request.Message,
                body = (string?)null,
                footer = (string?)null,
                isValid = false
            });
        }

        var parts = request.Message.Split(':', 2);
        var typeScope = parts[0].Trim();
        var subject = parts.Length > 1 ? parts[1].Trim() : "";

        string? type = null;
        string? scope = null;

        if (typeScope.Contains('('))
        {
            var typeScopeParts = typeScope.Split('(');
            type = typeScopeParts[0].Trim();
            scope = typeScopeParts[1].TrimEnd(')');
        }
        else
        {
            type = typeScope;
        }

        return Ok(new
        {
            hash = "abc123",
            type = type,
            scope = scope,
            subject = subject,
            body = (string?)null,
            footer = (string?)null,
            isValid = true
        });
    }

    [HttpGet("stats")]
    public IActionResult GetStats()
    {
        var stats = new
        {
            tests = new { total = 118, passed = 118, failed = 0 },
            coverage = new { domain = 100, application = 85, infrastructure = 70 },
            git = new { branches = 6, commits = 150, lastCommit = DateTime.UtcNow.AddHours(-2) }
        };

        return Ok(stats);
    }
}

public class CommitValidationRequest
{
    public string Message { get; set; } = string.Empty;
}

