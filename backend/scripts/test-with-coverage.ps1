# Script para executar testes com cobertura
# Gera relatórios em múltiplos formatos: OpenCover, Cobertura e JSON

param(
    [string]$Configuration = "Debug",
    [switch]$Verbose
)

Write-Host "Executando testes com cobertura..." -ForegroundColor Cyan

# Navegar para o diretório do projeto de testes
$testProjectPath = "..\tests\UnitTests\UnitTests.csproj"
if (-not (Test-Path $testProjectPath)) {
    Write-Host "ERRO: Projeto de testes não encontrado em $testProjectPath" -ForegroundColor Red
    exit 1
}

# Criar diretório de resultados se não existir
$resultsDir = "..\tests\UnitTests\TestResults"
if (-not (Test-Path $resultsDir)) {
    New-Item -ItemType Directory -Path $resultsDir -Force | Out-Null
}

# Executar testes com cobertura
Write-Host "`nExecutando testes..." -ForegroundColor Yellow

$testArgs = @(
    "test",
    $testProjectPath,
    "--configuration", $Configuration,
    "/p:CollectCoverage=true",
    "/p:CoverletOutputFormat=opencover;cobertura;json",
    "/p:CoverletOutput=$resultsDir\coverage",
    "/p:ExcludeByAttribute=Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute",
    "/p:Exclude=[*.Tests]*,[*.Test]*",
    "/p:IncludeDirectory=../src"
)

if ($Verbose) {
    $testArgs += "--verbosity", "detailed"
}

& dotnet $testArgs

if ($LASTEXITCODE -ne 0) {
    Write-Host "`nERRO: Testes falharam" -ForegroundColor Red
    exit $LASTEXITCODE
}

Write-Host "`nTestes executados com sucesso!" -ForegroundColor Green
Write-Host "`nRelatórios de cobertura gerados em:" -ForegroundColor Cyan
Write-Host "  - OpenCover: $resultsDir\coverage.opencover.xml" -ForegroundColor White
Write-Host "  - Cobertura: $resultsDir\coverage.cobertura.xml" -ForegroundColor White
Write-Host "  - JSON: $resultsDir\coverage.json" -ForegroundColor White

# Tentar exibir resumo se possível
$jsonPath = "$resultsDir\coverage.json"
if (Test-Path $jsonPath) {
    try {
        $coverage = Get-Content $jsonPath | ConvertFrom-Json
        $summary = $coverage.summary
        
        Write-Host "`nResumo de Cobertura:" -ForegroundColor Cyan
        Write-Host "  - Linhas: $($summary.linecoverage.ToString('P2'))" -ForegroundColor White
        Write-Host "  - Branches: $($summary.branchcoverage.ToString('P2'))" -ForegroundColor White
        Write-Host "  - Métodos: $($summary.methodcoverage.ToString('P2'))" -ForegroundColor White
    } catch {
        Write-Host "  (Não foi possível ler resumo do JSON)" -ForegroundColor Yellow
    }
}

Write-Host "`nPara visualizar relatórios HTML, use ferramentas como:" -ForegroundColor Cyan
Write-Host "  - ReportGenerator: dotnet tool install -g dotnet-reportgenerator-globaltool" -ForegroundColor White
Write-Host "  - Depois execute: reportgenerator -reports:$resultsDir\coverage.opencover.xml -targetdir:$resultsDir\html -reporttypes:Html" -ForegroundColor White

