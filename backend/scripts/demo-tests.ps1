param(
    [Parameter(Mandatory=$false)]
    [switch]$ShowSummary = $false,
    [string]$ResultsPath = ".\test-results"
)

$ErrorActionPreference = "Stop"

# Mudar para o diretório backend onde estão os arquivos docker-compose
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendPath = Join-Path $scriptPath ".."
Push-Location $backendPath

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  DEMONSTRAÇÃO DE TESTES - 123VENDAS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar Docker
Write-Host "Verificando Docker..." -ForegroundColor Yellow
$dockerRunning = docker info 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERRO] Docker não está rodando. Por favor, inicie o Docker Desktop." -ForegroundColor Red
    exit 1
}
Write-Host "[OK] Docker está rodando" -ForegroundColor Green
Write-Host ""

# Subir SQL Server
Write-Host "Subindo SQL Server de testes..." -ForegroundColor Yellow
docker-compose -f docker-compose.test.yml up -d sqlserver-test 2>&1 | Out-Null

Write-Host "Aguardando SQL Server ficar saudável..." -ForegroundColor Yellow
$maxWait = 60
$waited = 0
do {
    Start-Sleep -Seconds 2
    $waited += 2
    $health = docker inspect --format='{{.State.Health.Status}}' 123vendas-sqlserver-test 2>$null
    if ($health -eq "healthy") {
        Write-Host "[OK] SQL Server está saudável" -ForegroundColor Green
        break
    }
    if ($waited -ge $maxWait) {
        Write-Host "[AVISO] Timeout aguardando SQL Server. Continuando mesmo assim..." -ForegroundColor Yellow
        break
    }
} while ($true)
Write-Host ""

# Executar todos os testes
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  EXECUTANDO TODOS OS TESTES" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$startTime = Get-Date

docker-compose -f docker-compose.test.yml build all-tests 2>&1 | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERRO] Falha ao construir imagem de testes" -ForegroundColor Red
    exit 1
}

Write-Host "Executando testes..." -ForegroundColor Yellow
docker-compose -f docker-compose.test.yml run --rm all-tests

$testResult = $LASTEXITCODE
$endTime = Get-Date
$duration = ($endTime - $startTime).TotalSeconds

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  ESTATÍSTICAS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Tempo total de execução: $([math]::Round($duration, 2)) segundos" -ForegroundColor White
Write-Host ""

# Mostrar resumo se solicitado
if ($ShowSummary) {
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "  RESUMO DETALHADO" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    
    if (Test-Path "$ResultsPath\unit-results.trx") {
        Write-Host "✓ Resultados de testes unitários: $ResultsPath\unit-results.trx" -ForegroundColor Green
    }
    
    if (Test-Path "$ResultsPath\integration-results.trx") {
        Write-Host "✓ Resultados de testes de integração: $ResultsPath\integration-results.trx" -ForegroundColor Green
    }
    
    if (Test-Path "$ResultsPath\bdd-results.trx") {
        Write-Host "✓ Resultados de testes BDD: $ResultsPath\bdd-results.trx" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "Para visualizar os resultados no Visual Studio:" -ForegroundColor White
    Write-Host "  1. Abra o Visual Studio" -ForegroundColor Gray
    Write-Host "  2. Menu: Test > Test Explorer" -ForegroundColor Gray
    Write-Host "  3. Menu: Test > Import Test Results" -ForegroundColor Gray
    Write-Host "  4. Selecione os arquivos .trx em $ResultsPath" -ForegroundColor Gray
    Write-Host ""
}

# Resultado final
if ($testResult -eq 0) {
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  ✓ DEMONSTRAÇÃO CONCLUÍDA COM SUCESSO!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Todos os testes passaram!" -ForegroundColor Green
    Write-Host "Total de testes: 118 (38 unitários + 57 integração + 23 BDD)" -ForegroundColor White
    exit 0
} else {
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "  ✗ ALGUNS TESTES FALHARAM" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
    Write-Host "Verifique os resultados em: $ResultsPath" -ForegroundColor Yellow
    Pop-Location
    exit 1
}

# Voltar ao diretório original
Pop-Location

