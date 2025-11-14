# Script para parar o ambiente 123Vendas
# Autor: Sistema de Testes
# Data: 2025-11-14

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  123Vendas - Parando Ambiente" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Verificar se há containers rodando
Write-Host "[1/3] Verificando containers..." -ForegroundColor Yellow
$containers = docker ps -a --filter "name=123vendas" --format "{{.Names}}"

if ($containers.Count -eq 0) {
    Write-Host "  ℹ Nenhum container encontrado" -ForegroundColor Gray
    exit 0
}

Write-Host "  ✓ Encontrados $($containers.Count) container(s)" -ForegroundColor Green

# Parar containers
Write-Host "[2/3] Parando containers..." -ForegroundColor Yellow
docker-compose down
Write-Host "  ✓ Containers parados" -ForegroundColor Green

# Perguntar se deseja remover volumes
Write-Host "[3/3] Limpeza de volumes..." -ForegroundColor Yellow
$removeVolumes = Read-Host "  Deseja remover volumes? Isso apagará os dados do banco (S/N)"

if ($removeVolumes -eq "S" -or $removeVolumes -eq "s") {
    docker-compose down -v
    Write-Host "  ✓ Volumes removidos" -ForegroundColor Green
} else {
    Write-Host "  ℹ Volumes mantidos (dados preservados)" -ForegroundColor Gray
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Ambiente Parado com Sucesso!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Para iniciar novamente, execute: .\scripts\start-environment.ps1" -ForegroundColor Gray
Write-Host ""

