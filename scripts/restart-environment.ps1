# Script para reiniciar o ambiente 123Vendas
# Autor: Sistema de Testes
# Data: 2025-11-14

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  123Vendas - Reiniciando Ambiente" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Perguntar se deseja rebuild
$rebuild = Read-Host "Deseja fazer rebuild das imagens? (S/N)"

if ($rebuild -eq "S" -or $rebuild -eq "s") {
    Write-Host "Reiniciando com rebuild..." -ForegroundColor Yellow
    docker-compose down
    docker-compose up -d --build
} else {
    Write-Host "Reiniciando sem rebuild..." -ForegroundColor Yellow
    docker-compose restart
}

Write-Host ""
Write-Host "Aguardando serviços reiniciarem..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Verificar status
Write-Host ""
Write-Host "Status dos serviços:" -ForegroundColor Yellow
docker-compose ps --format "table {{.Name}}\t{{.Status}}\t{{.Ports}}"

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Ambiente Reiniciado!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

