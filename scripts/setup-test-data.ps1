# Script para popular banco de dados com massa de dados de teste
# Cria vendas com diferentes cenários para facilitar testes

param(
    [string]$BaseUrl = "http://localhost:5000",
    [switch]$CleanDatabase
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  SETUP DE DADOS DE TESTE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# IDs conhecidos para referência
$global:TestData = @{
    VendaIds = @{}
    ClienteIds = @{}
    FilialIds = @{}
    ProdutoIds = @{}
}

function New-Guid {
    return [guid]::NewGuid().ToString()
}

function Create-Venda {
    param(
        [string]$NumeroVenda,
        [string]$ClienteNome,
        [string]$FilialNome,
        [array]$Itens,
        [bool]$Cancelar = $false
    )
    
    $clienteId = New-Guid
    $filialId = New-Guid
    
    $venda = @{
        numeroVenda = $NumeroVenda
        clienteId = $clienteId
        clienteNome = $ClienteNome
        filialId = $filialId
        filialNome = $FilialNome
        itens = $Itens
    }
    
    try {
        $headers = @{
            "Content-Type" = "application/json"
            "Accept" = "application/json"
        }
        
        $body = $venda | ConvertTo-Json -Depth 10
        $response = Invoke-RestMethod -Uri "$BaseUrl/api/venda" -Method POST -Headers $headers -Body $body -ErrorAction Stop
        
        if ($response.success -and $response.data) {
            $vendaId = $response.data
            
            # Cancelar se solicitado
            if ($Cancelar) {
                try {
                    Invoke-RestMethod -Uri "$BaseUrl/api/venda/$vendaId" -Method DELETE -ErrorAction Stop | Out-Null
                    Write-Host "  ✅ Venda $NumeroVenda criada e cancelada (ID: $vendaId)" -ForegroundColor Green
                } catch {
                    Write-Host "  ⚠️  Venda $NumeroVenda criada mas falhou ao cancelar" -ForegroundColor Yellow
                }
            } else {
                Write-Host "  ✅ Venda $NumeroVenda criada (ID: $vendaId)" -ForegroundColor Green
            }
            
            $global:TestData.VendaIds[$NumeroVenda] = $vendaId
            return $vendaId
        }
    }
    catch {
        Write-Host "  ❌ Falha ao criar venda $NumeroVenda : $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

# Limpar banco (se solicitado)
if ($CleanDatabase) {
    Write-Host "⚠️  Limpeza de banco não implementada via API" -ForegroundColor Yellow
    Write-Host "   Use o script de testes para criar dados limpos" -ForegroundColor Gray
}

Write-Host "Criando massa de dados de teste..." -ForegroundColor Yellow
Write-Host ""

# Venda V001: 3 itens, sem desconto, não cancelada
Write-Host "1. Criando V001 (3 itens, sem desconto)..." -ForegroundColor Cyan
$v001Itens = @(
    @{
        produtoId = New-Guid
        produtoNome = "Produto A - Teste"
        quantidade = 3
        valorUnitario = 100.00
    }
)
Create-Venda -NumeroVenda "V001" -ClienteNome "Cliente Teste 1" -FilialNome "Filial Teste 1" -Itens $v001Itens

# Venda V002: 5 itens, 10% desconto, não cancelada
Write-Host "2. Criando V002 (5 itens, 10% desconto)..." -ForegroundColor Cyan
$v002Itens = @(
    @{
        produtoId = New-Guid
        produtoNome = "Produto B - Teste"
        quantidade = 5
        valorUnitario = 100.00
    }
)
Create-Venda -NumeroVenda "V002" -ClienteNome "Cliente Teste 2" -FilialNome "Filial Teste 2" -Itens $v002Itens

# Venda V003: 15 itens, 20% desconto, não cancelada
Write-Host "3. Criando V003 (15 itens, 20% desconto)..." -ForegroundColor Cyan
$v003Itens = @(
    @{
        produtoId = New-Guid
        produtoNome = "Produto C - Teste"
        quantidade = 15
        valorUnitario = 100.00
    }
)
Create-Venda -NumeroVenda "V003" -ClienteNome "Cliente Teste 3" -FilialNome "Filial Teste 3" -Itens $v003Itens

# Venda V004: Múltiplos itens, não cancelada
Write-Host "4. Criando V004 (múltiplos itens)..." -ForegroundColor Cyan
$v004Itens = @(
    @{
        produtoId = New-Guid
        produtoNome = "Produto D1 - Teste"
        quantidade = 3
        valorUnitario = 50.00
    },
    @{
        produtoId = New-Guid
        produtoNome = "Produto D2 - Teste"
        quantidade = 5
        valorUnitario = 100.00
    },
    @{
        produtoId = New-Guid
        produtoNome = "Produto D3 - Teste"
        quantidade = 10
        valorUnitario = 200.00
    }
)
Create-Venda -NumeroVenda "V004" -ClienteNome "Cliente Teste 4" -FilialNome "Filial Teste 4" -Itens $v004Itens

# Venda V005: 3 itens, cancelada
Write-Host "5. Criando V005 (3 itens, cancelada)..." -ForegroundColor Cyan
$v005Itens = @(
    @{
        produtoId = New-Guid
        produtoNome = "Produto E - Teste"
        quantidade = 3
        valorUnitario = 100.00
    }
)
Create-Venda -NumeroVenda "V005" -ClienteNome "Cliente Teste 5" -FilialNome "Filial Teste 5" -Itens $v005Itens -Cancelar $true

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  RESUMO DO SETUP" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Total de vendas criadas: $($global:TestData.VendaIds.Count)" -ForegroundColor White
Write-Host ""

if ($global:TestData.VendaIds.Count -gt 0) {
    Write-Host "IDs das vendas criadas:" -ForegroundColor Cyan
    foreach ($key in $global:TestData.VendaIds.Keys) {
        Write-Host "  $key : $($global:TestData.VendaIds[$key])" -ForegroundColor Gray
    }
    
    # Salvar IDs em arquivo para referência
    $idsFile = Join-Path $PSScriptRoot "test-data-ids.json"
    $global:TestData | ConvertTo-Json -Depth 10 | Out-File -FilePath $idsFile -Encoding UTF8
    Write-Host ""
    Write-Host "✅ IDs salvos em: $idsFile" -ForegroundColor Green
}

Write-Host ""
Write-Host "✅ Setup de dados de teste concluído!" -ForegroundColor Green

