param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("unit", "integration", "bdd", "all")]
    [string]$TestType = "all",
    [string]$ResultsPath = ".\test-results"
)

$ErrorActionPreference = "Stop"

# Mudar para o diretório backend onde estão os arquivos docker-compose
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendPath = Join-Path $scriptPath ".."
Push-Location $backendPath

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  EXECUTAR TESTES NO DOCKER - 123VENDAS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

if (-not (Test-Path $ResultsPath)) {
    New-Item -Path $ResultsPath -ItemType Directory | Out-Null
    Write-Host "[OK] Diretório test-results criado" -ForegroundColor Green
}

Write-Host "Verificando Docker..." -ForegroundColor Yellow
$dockerRunning = docker info 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "[FAIL] Docker não está rodando. Por favor, inicie o Docker Desktop." -ForegroundColor Red
    exit 1
}
Write-Host "[OK] Docker está rodando" -ForegroundColor Green

Write-Host "`nSubindo SQL Server de testes..." -ForegroundColor Yellow
docker-compose -f docker-compose.test.yml up -d sqlserver-test

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
        Write-Host "[WARN] Timeout aguardando SQL Server. Continuando mesmo assim..." -ForegroundColor Yellow
        break
    }
} while ($true)

function Run-UnitTests {
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "  EXECUTANDO TESTES UNITÁRIOS" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    
    docker-compose -f docker-compose.test.yml build unit-tests
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[FAIL] Erro ao construir imagem de testes unitários" -ForegroundColor Red
        return $false
    }
    
    docker-compose -f docker-compose.test.yml run --rm unit-tests
    $unitResult = $LASTEXITCODE
    
    if ($unitResult -eq 0) {
        Write-Host "[OK] Testes unitários passaram" -ForegroundColor Green
        return $true
    } else {
        Write-Host "[FAIL] Testes unitários falharam" -ForegroundColor Red
        return $false
    }
}

function Run-IntegrationTests {
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "  EXECUTANDO TESTES DE INTEGRAÇÃO" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    
    docker-compose -f docker-compose.test.yml build integration-tests
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[FAIL] Erro ao construir imagem de testes de integração" -ForegroundColor Red
        return $false
    }
    
    docker-compose -f docker-compose.test.yml run --rm integration-tests
    $integrationResult = $LASTEXITCODE
    
    if ($integrationResult -eq 0) {
        Write-Host "[OK] Testes de integração passaram" -ForegroundColor Green
        return $true
    } else {
        Write-Host "[FAIL] Testes de integração falharam" -ForegroundColor Red
        return $false
    }
}

function Run-BddTests {
    Write-Host "`n========================================" -ForegroundColor Cyan
    Write-Host "  EXECUTANDO TESTES BDD" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    
    docker-compose -f docker-compose.test.yml build bdd-tests
    if ($LASTEXITCODE -ne 0) {
        Write-Host "[FAIL] Erro ao construir imagem de testes BDD" -ForegroundColor Red
        return $false
    }
    
    docker-compose -f docker-compose.test.yml run --rm bdd-tests
    $bddResult = $LASTEXITCODE
    
    if ($bddResult -eq 0) {
        Write-Host "[OK] Testes BDD passaram" -ForegroundColor Green
        return $true
    } else {
        Write-Host "[FAIL] Testes BDD falharam" -ForegroundColor Red
        return $false
    }
}

$results = @{
    Unit = $false
    Integration = $false
    Bdd = $false
}

switch ($TestType) {
    "unit" {
        $results.Unit = Run-UnitTests
    }
    "integration" {
        $results.Integration = Run-IntegrationTests
    }
    "bdd" {
        $results.Bdd = Run-BddTests
    }
    "all" {
        $results.Unit = Run-UnitTests
        $results.Integration = Run-IntegrationTests
        $results.Bdd = Run-BddTests
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  RESUMO DOS TESTES" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

if ($TestType -eq "all" -or $TestType -eq "unit") {
    $status = if ($results.Unit) { "[OK]" } else { "[FAIL]" }
    $color = if ($results.Unit) { "Green" } else { "Red" }
    Write-Host "Testes Unitários: $status" -ForegroundColor $color
}

if ($TestType -eq "all" -or $TestType -eq "integration") {
    $status = if ($results.Integration) { "[OK]" } else { "[FAIL]" }
    $color = if ($results.Integration) { "Green" } else { "Red" }
    Write-Host "Testes de Integração: $status" -ForegroundColor $color
}

if ($TestType -eq "all" -or $TestType -eq "bdd") {
    $status = if ($results.Bdd) { "[OK]" } else { "[FAIL]" }
    $color = if ($results.Bdd) { "Green" } else { "Red" }
    Write-Host "Testes BDD: $status" -ForegroundColor $color
}

Write-Host "`nResultados salvos em: $ResultsPath" -ForegroundColor White

$allPassed = ($results.Unit -and $results.Integration -and $results.Bdd) -or 
             ($TestType -ne "all" -and ($results.Unit -or $results.Integration -or $results.Bdd))

if ($allPassed) {
    Write-Host "`n[OK] Todos os testes executados com sucesso!" -ForegroundColor Green
    Pop-Location
    exit 0
} else {
    Write-Host "`n[FAIL] Alguns testes falharam. Verifique os resultados em $ResultsPath" -ForegroundColor Yellow
    Pop-Location
    exit 1
}

