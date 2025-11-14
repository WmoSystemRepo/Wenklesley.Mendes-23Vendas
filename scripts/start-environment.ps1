# Script para iniciar o ambiente completo 123Vendas
# Autor: Sistema de Testes
# Data: 2025-11-14

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  123Vendas - Iniciando Ambiente" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se Docker está instalado e rodando
Write-Host "[1/6] Verificando Docker..." -ForegroundColor Yellow
try {
    $dockerVersion = docker --version
    Write-Host "  ✓ Docker encontrado: $dockerVersion" -ForegroundColor Green
} catch {
    Write-Host "  ✗ Docker não encontrado. Por favor, instale o Docker Desktop." -ForegroundColor Red
    exit 1
}

# Verificar se Docker está rodando
try {
    docker ps | Out-Null
    Write-Host "  ✓ Docker está rodando" -ForegroundColor Green
} catch {
    Write-Host "  ✗ Docker não está rodando. Por favor, inicie o Docker Desktop." -ForegroundColor Red
    exit 1
}

# Verificar portas disponíveis
Write-Host "[2/6] Verificando portas disponíveis..." -ForegroundColor Yellow
$ports = @(1433, 5000, 4200)
$portsInUse = @()

foreach ($port in $ports) {
    $connection = Test-NetConnection -ComputerName localhost -Port $port -InformationLevel Quiet -WarningAction SilentlyContinue
    if ($connection) {
        $portsInUse += $port
        Write-Host "  ⚠ Porta $port está em uso" -ForegroundColor Yellow
    } else {
        Write-Host "  ✓ Porta $port disponível" -ForegroundColor Green
    }
}

if ($portsInUse.Count -gt 0) {
    Write-Host "  ⚠ Algumas portas estão em uso. Os containers podem não iniciar corretamente." -ForegroundColor Yellow
    $continue = Read-Host "  Deseja continuar mesmo assim? (S/N)"
    if ($continue -ne "S" -and $continue -ne "s") {
        exit 0
    }
}

# Parar containers existentes (se houver)
Write-Host "[3/6] Parando containers existentes..." -ForegroundColor Yellow
docker-compose down 2>&1 | Out-Null
Write-Host "  ✓ Containers parados" -ForegroundColor Green

# Subir containers
Write-Host "[4/6] Subindo containers (isso pode levar alguns minutos)..." -ForegroundColor Yellow
Write-Host "  Aguarde enquanto os containers são construídos e iniciados..." -ForegroundColor Gray

$buildOutput = docker-compose up -d --build 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "  ✗ Erro ao subir containers" -ForegroundColor Red
    Write-Host $buildOutput -ForegroundColor Red
    exit 1
}

Write-Host "  ✓ Containers iniciados" -ForegroundColor Green

# Aguardar serviços ficarem prontos
Write-Host "[5/6] Aguardando serviços ficarem prontos..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

$maxAttempts = 30
$attempt = 0
$allReady = $false

while ($attempt -lt $maxAttempts -and -not $allReady) {
    $attempt++
    Write-Host "  Tentativa $attempt/$maxAttempts..." -ForegroundColor Gray
    
    $apiReady = $false
    $frontendReady = $false
    
    # Verificar API
    try {
        $healthResponse = Invoke-RestMethod -Uri "http://localhost:5000/health" -Method Get -TimeoutSec 2 -ErrorAction Stop
        if ($healthResponse.status -eq "healthy") {
            $apiReady = $true
        }
    } catch {
        # API ainda não está pronta
    }
    
    # Verificar Frontend
    try {
        $frontendResponse = Invoke-WebRequest -Uri "http://localhost:4200" -UseBasicParsing -TimeoutSec 2 -ErrorAction Stop
        if ($frontendResponse.StatusCode -eq 200) {
            $frontendReady = $true
        }
    } catch {
        # Frontend ainda não está pronto
    }
    
    if ($apiReady -and $frontendReady) {
        $allReady = $true
    } else {
        Start-Sleep -Seconds 5
    }
}

if (-not $allReady) {
    Write-Host "  ⚠ Alguns serviços podem ainda estar inicializando" -ForegroundColor Yellow
} else {
    Write-Host "  ✓ Todos os serviços estão prontos" -ForegroundColor Green
}

# Verificar status final
Write-Host "[6/6] Verificando status dos serviços..." -ForegroundColor Yellow
Write-Host ""
docker-compose ps --format "table {{.Name}}\t{{.Status}}\t{{.Ports}}"
Write-Host ""

# Testar endpoints
Write-Host "Testando endpoints..." -ForegroundColor Yellow

# Health Check
try {
    $health = Invoke-RestMethod -Uri "http://localhost:5000/health" -Method Get -ErrorAction Stop
    Write-Host "  ✓ API Health: $($health.status)" -ForegroundColor Green
} catch {
    Write-Host "  ✗ API Health Check falhou" -ForegroundColor Red
}

# Frontend
try {
    $frontend = Invoke-WebRequest -Uri "http://localhost:4200" -UseBasicParsing -ErrorAction Stop
    Write-Host "  ✓ Frontend: Status $($frontend.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "  ✗ Frontend não responde" -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Ambiente Iniciado com Sucesso!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "URLs de Acesso:" -ForegroundColor Yellow
Write-Host "  • Frontend Dashboard: http://localhost:4200" -ForegroundColor White
Write-Host "  • API Swagger:        http://localhost:5000/swagger" -ForegroundColor White
Write-Host "  • API Health Check:    http://localhost:5000/health" -ForegroundColor White
Write-Host ""
Write-Host "Próximos Passos:" -ForegroundColor Yellow
Write-Host "  1. Acesse o dashboard em http://localhost:4200" -ForegroundColor White
Write-Host "  2. Consulte o ROTEIRO_TESTES.md para testar o sistema" -ForegroundColor White
Write-Host ""
Write-Host "Para parar o ambiente, execute: .\scripts\stop-environment.ps1" -ForegroundColor Gray
Write-Host ""

