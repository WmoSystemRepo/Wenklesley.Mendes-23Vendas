# Script principal para executar todos os testes da API 123Vendas
# Orquestra todo o processo: verificação, setup de dados, execução de testes e geração de relatórios

param(
    [string]$BaseUrl = "http://localhost:5000",
    [switch]$SetupData,
    [switch]$CleanDatabase,
    [string]$OutputPath = "./reports"
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  EXECUTOR DE TESTES - API 123VENDAS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se o diretório de relatórios existe
if (-not (Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
    Write-Host "Diretório de relatórios criado: $OutputPath" -ForegroundColor Gray
}

# Função para verificar se a API está rodando
function Test-ApiHealth {
    param([string]$Url)
    
    try {
        $response = Invoke-RestMethod -Uri "$Url/health" -Method GET -TimeoutSec 5 -ErrorAction Stop
        if ($response.status -eq "healthy") {
            Write-Host "✅ API está respondendo corretamente" -ForegroundColor Green
            return $true
        }
    }
    catch {
        Write-Host "❌ API não está respondendo em $Url" -ForegroundColor Red
        Write-Host "   Erro: $($_.Exception.Message)" -ForegroundColor Yellow
        return $false
    }
    return $false
}

# Verificar API
Write-Host "Verificando se a API está rodando..." -ForegroundColor Yellow
if (-not (Test-ApiHealth -Url $BaseUrl)) {
    Write-Host "`n⚠️  A API não está respondendo. Certifique-se de que ela está rodando em $BaseUrl" -ForegroundColor Yellow
    Write-Host "   Execute: docker-compose up -d" -ForegroundColor Gray
    exit 1
}

# Setup de dados de teste (se solicitado)
if ($SetupData) {
    Write-Host "`nConfigurando massa de dados de teste..." -ForegroundColor Yellow
    $setupScript = Join-Path $PSScriptRoot "setup-test-data.ps1"
    if (Test-Path $setupScript) {
        & $setupScript -BaseUrl $BaseUrl -CleanDatabase:$CleanDatabase
    } else {
        Write-Host "⚠️  Script de setup não encontrado: $setupScript" -ForegroundColor Yellow
    }
}

# Executar testes
Write-Host "`nExecutando testes..." -ForegroundColor Yellow
$testScript = Join-Path $PSScriptRoot "run-tests-complete.ps1"
if (-not (Test-Path $testScript)) {
    Write-Host "❌ Script de testes não encontrado: $testScript" -ForegroundColor Red
    exit 1
}

# Gerar timestamp para nomes de arquivo
$timestamp = Get-Date -Format "yyyyMMdd-HHmmss"
$jsonReportPath = Join-Path $OutputPath "test-results-$timestamp.json"
$mdReportPath = Join-Path $OutputPath "test-results-$timestamp.md"

# Executar testes e capturar resultado
try {
    $testResults = & $testScript -BaseUrl $BaseUrl
} catch {
    Write-Host "❌ Erro ao executar testes: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Salvar relatório JSON
if ($testResults -and $testResults.testRun) {
    $testResults | ConvertTo-Json -Depth 10 | Out-File -FilePath $jsonReportPath -Encoding UTF8
    Write-Host "`n✅ Relatório JSON salvo: $jsonReportPath" -ForegroundColor Green
    
    # Gerar relatório Markdown
    $reportScript = Join-Path $PSScriptRoot "generate-report.ps1"
    if (Test-Path $reportScript) {
        & $reportScript -JsonPath $jsonReportPath -OutputPath $mdReportPath
        Write-Host "✅ Relatório Markdown salvo: $mdReportPath" -ForegroundColor Green
    } else {
        Write-Host "⚠️  Script de geração de relatório Markdown não encontrado" -ForegroundColor Yellow
    }
    
    # Exibir resumo
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "  RESUMO DA EXECUÇÃO" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Total de testes: $($testResults.testRun.totalTests)" -ForegroundColor White
    Write-Host "✅ Passou: $($testResults.testRun.passed)" -ForegroundColor Green
    Write-Host "❌ Falhou: $($testResults.testRun.failed)" -ForegroundColor Red
    Write-Host "Taxa de sucesso: $($testResults.testRun.successRate)%" -ForegroundColor $(if ($testResults.testRun.successRate -ge 90) { "Green" } else { "Yellow" })
    Write-Host ""
    Write-Host "Relatórios gerados:" -ForegroundColor Cyan
    Write-Host "  - JSON: $jsonReportPath" -ForegroundColor Gray
    Write-Host "  - Markdown: $mdReportPath" -ForegroundColor Gray
    Write-Host ""
    
    if ($testResults.testRun.failed -eq 0) {
        Write-Host "🎉 Todos os testes passaram!" -ForegroundColor Green
        exit 0
    } else {
        Write-Host "⚠️  Alguns testes falharam. Verifique os relatórios para detalhes." -ForegroundColor Yellow
        exit 1
    }
} else {
    Write-Host "❌ Falha ao executar testes" -ForegroundColor Red
    exit 1
}

