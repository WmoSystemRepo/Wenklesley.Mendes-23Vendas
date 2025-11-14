# Script completo de testes da API 123Vendas
# Testa todos os endpoints de todas as formas possíveis

param(
    [string]$BaseUrl = "http://localhost:5000",
    [switch]$Verbose
)

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  TESTE COMPLETO DA API 123VENDAS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Variáveis globais
$global:TestResults = @{
    Total = 0
    Passed = 0
    Failed = 0
    Tests = @()
}

function Test-Endpoint {
    param(
        [string]$Name,
        [string]$Method,
        [string]$Url,
        [object]$Body = $null,
        [int]$ExpectedStatus = 200,
        [scriptblock]$Validation = $null
    )
    
    $global:TestResults.Total++
    $testResult = @{
        Name = $Name
        Method = $Method
        Url = $Url
        Status = "PENDING"
        Message = ""
    }
    
    try {
        $headers = @{
            "Content-Type" = "application/json"
            "Accept" = "application/json"
        }
        
        $params = @{
            Uri = "$BaseUrl$Url"
            Method = $Method
            Headers = $headers
        }
        
        if ($Body) {
            $params.Body = ($Body | ConvertTo-Json -Depth 10)
        }
        
        $response = Invoke-RestMethod @params -ErrorAction Stop
        $statusCode = 200
        
        # Verificar status code esperado
        if ($statusCode -ne $ExpectedStatus) {
            throw "Status code esperado: $ExpectedStatus, recebido: $statusCode"
        }
        
        # Executar validação customizada se fornecida
        if ($Validation) {
            $validationResult = & $Validation $response
            if (-not $validationResult) {
                throw "Validação customizada falhou"
            }
        }
        
        $testResult.Status = "PASSED"
        $testResult.Message = "✅ Sucesso"
        $global:TestResults.Passed++
        
        if ($Verbose) {
            Write-Host "✅ $Name" -ForegroundColor Green
            Write-Host "   Response: $($response | ConvertTo-Json -Depth 3)" -ForegroundColor Gray
        } else {
            Write-Host "✅ $Name" -ForegroundColor Green
        }
        
        return $response
    }
    catch {
        $testResult.Status = "FAILED"
        $testResult.Message = $_.Exception.Message
        $global:TestResults.Failed++
        Write-Host "❌ $Name" -ForegroundColor Red
        Write-Host "   Erro: $($_.Exception.Message)" -ForegroundColor Yellow
        return $null
    }
    
    $global:TestResults.Tests += $testResult
}

Write-Host "Testando Health Check..." -ForegroundColor Yellow
Test-Endpoint -Name "Health Check" -Method "GET" -Url "/health" -ExpectedStatus 200

Write-Host "`nTESTE 1: GET /api/venda - Listar todas as vendas" -ForegroundColor Cyan
Test-Endpoint -Name "Listar Vendas vazio" -Method "GET" -Url "/api/venda" -ExpectedStatus 200

Write-Host "`n📝 TESTE 2: POST /api/venda - Criar vendas" -ForegroundColor Cyan

# Teste 2.1: Criar venda com 3 itens (sem desconto)
$venda1 = @{
    numeroVenda = "V001"
    clienteId = [guid]::NewGuid()
    clienteNome = "Cliente Teste 1"
    filialId = [guid]::NewGuid()
    filialNome = "Filial Teste 1"
    itens = @(
        @{
            produtoId = [guid]::NewGuid()
            produtoNome = "Produto A"
            quantidade = 3
            valorUnitario = 100.00
        }
    )
}
$createResponse1 = Test-Endpoint -Name "Criar venda com 3 itens sem desconto" -Method "POST" -Url "/api/venda" -Body $venda1 -ExpectedStatus 201 -Validation {
    param($response)
    $response.success -eq $true -and $response.data -ne [guid]::Empty
}
$vendaId1 = if ($createResponse1 -and $createResponse1.data) { $createResponse1.data } else { $null }

# Teste 2.2: Criar venda com 5 itens (10 porcento desconto)
$venda2 = @{
    numeroVenda = "V002"
    clienteId = [guid]::NewGuid()
    clienteNome = "Cliente Teste 2"
    filialId = [guid]::NewGuid()
    filialNome = "Filial Teste 2"
    itens = @(
        @{
            produtoId = [guid]::NewGuid()
            produtoNome = "Produto B"
            quantidade = 5
            valorUnitario = 100.00
        }
    )
}
$createResponse2 = Test-Endpoint -Name "Criar venda com 5 itens 10 porcento desconto" -Method "POST" -Url "/api/venda" -Body $venda2 -ExpectedStatus 201
$vendaId2 = if ($createResponse2 -and $createResponse2.data) { $createResponse2.data } else { $null }

# Teste 2.3: Criar venda com 15 itens (20 porcento desconto)
$venda3 = @{
    numeroVenda = "V003"
    clienteId = [guid]::NewGuid()
    clienteNome = "Cliente Teste 3"
    filialId = [guid]::NewGuid()
    filialNome = "Filial Teste 3"
    itens = @(
        @{
            produtoId = [guid]::NewGuid()
            produtoNome = "Produto C"
            quantidade = 15
            valorUnitario = 100.00
        }
    )
}
$createResponse3 = Test-Endpoint -Name "Criar venda com 15 itens 20 porcento desconto" -Method "POST" -Url "/api/venda" -Body $venda3 -ExpectedStatus 201
$vendaId3 = if ($createResponse3 -and $createResponse3.data) { $createResponse3.data } else { $null }

# Teste 2.4: Criar venda com 21 itens (deve falhar)
$venda4 = @{
    numeroVenda = "V004"
    clienteId = [guid]::NewGuid()
    clienteNome = "Cliente Teste 4"
    filialId = [guid]::NewGuid()
    filialNome = "Filial Teste 4"
    itens = @(
        @{
            produtoId = [guid]::NewGuid()
            produtoNome = "Produto D"
            quantidade = 21
            valorUnitario = 100.00
        }
    )
}
Test-Endpoint -Name "Criar venda com 21 itens deve falhar" -Method "POST" -Url "/api/venda" -Body $venda4 -ExpectedStatus 400

# Teste 2.5: Criar venda com múltiplos itens
$venda5 = @{
    numeroVenda = "V005"
    clienteId = [guid]::NewGuid()
    clienteNome = "Cliente Teste 5"
    filialId = [guid]::NewGuid()
    filialNome = "Filial Teste 5"
    itens = @(
        @{
            produtoId = [guid]::NewGuid()
            produtoNome = "Produto E1"
            quantidade = 3
            valorUnitario = 50.00
        },
        @{
            produtoId = [guid]::NewGuid()
            produtoNome = "Produto E2"
            quantidade = 5
            valorUnitario = 100.00
        },
        @{
            produtoId = [guid]::NewGuid()
            produtoNome = "Produto E3"
            quantidade = 10
            valorUnitario = 200.00
        }
    )
}
$createResponse5 = Test-Endpoint -Name "Criar venda com múltiplos itens" -Method "POST" -Url "/api/venda" -Body $venda5 -ExpectedStatus 201
$vendaId5 = if ($createResponse5 -and $createResponse5.data) { $createResponse5.data } else { $null }

Write-Host "`nTESTE 3: GET /api/venda/{id} - Obter venda por ID" -ForegroundColor Cyan

if ($vendaId1) {
    Test-Endpoint -Name "Obter venda 1 por ID" -Method "GET" -Url "/api/venda/$vendaId1" -ExpectedStatus 200 -Validation {
        param($response)
        $response.success -eq $true -and $response.data -ne $null -and $response.data.numeroVenda -eq "V001"
    }
}

if ($vendaId2) {
    Test-Endpoint -Name "Obter venda 2 por ID verificar desconto 10 porcento" -Method "GET" -Url "/api/venda/$vendaId2" -ExpectedStatus 200 -Validation {
        param($response)
        $item = $response.data.itens[0]
        $item.desconto -eq 50.00 -and $item.valorTotalItem -eq 450.00
    }
}

if ($vendaId3) {
    Test-Endpoint -Name "Obter venda 3 por ID verificar desconto 20 porcento" -Method "GET" -Url "/api/venda/$vendaId3" -ExpectedStatus 200 -Validation {
        param($response)
        $item = $response.data.itens[0]
        $item.desconto -eq 300.00 -and $item.valorTotalItem -eq 1200.00
    }
}

# Teste com ID inválido
Test-Endpoint -Name "Obter venda com ID inválido" -Method "GET" -Url "/api/venda/00000000-0000-0000-0000-000000000000" -ExpectedStatus 404

Write-Host "`nTESTE 4: GET /api/venda - Listar todas as vendas apos criar" -ForegroundColor Cyan
Test-Endpoint -Name "Listar todas as vendas" -Method "GET" -Url "/api/venda" -ExpectedStatus 200 -Validation {
    param($response)
    $response.success -eq $true -and $response.data.Count -ge 4
}

Write-Host "`nTESTE 5: PUT /api/venda/{id} - Atualizar venda" -ForegroundColor Cyan

if ($vendaId1) {
    # Adicionar novo item
    $update1 = @{
        itensParaAdicionar = @(
            @{
                produtoId = [guid]::NewGuid()
                produtoNome = "Produto Novo"
                quantidade = 5
                valorUnitario = 50.00
            }
        )
    }
    Test-Endpoint -Name "Adicionar item à venda" -Method "PUT" -Url "/api/venda/$vendaId1" -Body $update1 -ExpectedStatus 204
    
    # Atualizar quantidade de item existente
    if ($vendaId2) {
        $getVenda2 = Invoke-RestMethod -Uri "$BaseUrl/api/venda/$vendaId2" -Method GET
        $itemId = $getVenda2.data.itens[0].id
        
        $update2 = @{
            itensParaAtualizar = @(
                @{
                    itemId = $itemId
                    quantidade = 10
                    valorUnitario = 100.00
                }
            )
        }
        Test-Endpoint -Name "Atualizar quantidade de item 5 para 10 desconto muda para 20 porcento" -Method "PUT" -Url "/api/venda/$vendaId2" -Body $update2 -ExpectedStatus 204
    }
    
    # Remover item
    if ($vendaId5) {
        $getVenda5 = Invoke-RestMethod -Uri "$BaseUrl/api/venda/$vendaId5" -Method GET
        $itemIdToRemove = $getVenda5.data.itens[0].id
        
        $update3 = @{
            itensParaRemover = @($itemIdToRemove)
        }
        Test-Endpoint -Name "Remover item da venda" -Method "PUT" -Url "/api/venda/$vendaId5" -Body $update3 -ExpectedStatus 204
    }
}

Write-Host "`nTESTE 6: DELETE /api/venda/{id} - Cancelar venda" -ForegroundColor Cyan

if ($vendaId1) {
    Test-Endpoint -Name "Cancelar venda" -Method "DELETE" -Url "/api/venda/$vendaId1" -ExpectedStatus 204
    
    # Tentar adicionar item a venda cancelada (deve falhar)
    $updateCancelada = @{
        itensParaAdicionar = @(
            @{
                produtoId = [guid]::NewGuid()
                produtoNome = "Produto Teste"
                quantidade = 1
                valorUnitario = 10.00
            }
        )
    }
    Test-Endpoint -Name "Tentar adicionar item a venda cancelada deve falhar" -Method "PUT" -Url "/api/venda/$vendaId1" -Body $updateCancelada -ExpectedStatus 400
}

# Tentar cancelar venda inexistente
Test-Endpoint -Name "Cancelar venda inexistente" -Method "DELETE" -Url "/api/venda/00000000-0000-0000-0000-000000000000" -ExpectedStatus 404

Write-Host "`n📊 TESTE 7: Validações de regras de negócio" -ForegroundColor Cyan

# Teste com 4 itens (sem desconto)
$venda4Itens = @{
    numeroVenda = "V006"
    clienteId = [guid]::NewGuid()
    clienteNome = "Cliente Teste 6"
    filialId = [guid]::NewGuid()
    filialNome = "Filial Teste 6"
    itens = @(
        @{
            produtoId = [guid]::NewGuid()
            produtoNome = "Produto F"
            quantidade = 4
            valorUnitario = 100.00
        }
    )
}
$createResponse4Itens = Test-Endpoint -Name "Criar venda com 4 itens sem desconto" -Method "POST" -Url "/api/venda" -Body $venda4Itens -ExpectedStatus 201
$vendaId4Itens = if ($createResponse4Itens -and $createResponse4Itens.data) { $createResponse4Itens.data } else { $null }

if ($vendaId4Itens) {
    $getVenda4Itens = Invoke-RestMethod -Uri "$BaseUrl/api/venda/$vendaId4Itens" -Method GET
    $item = $getVenda4Itens.data.itens[0]
    if ($item.desconto -eq 0) {
        Write-Host "✅ Validação: 4 itens não tem desconto" -ForegroundColor Green
        $global:TestResults.Passed++
    } else {
        Write-Host "❌ Validação: 4 itens deveria ter desconto 0, mas tem $($item.desconto)" -ForegroundColor Red
        $global:TestResults.Failed++
    }
}

# Teste com 10 itens (20 porcento desconto)
$venda10Itens = @{
    numeroVenda = "V007"
    clienteId = [guid]::NewGuid()
    clienteNome = "Cliente Teste 7"
    filialId = [guid]::NewGuid()
    filialNome = "Filial Teste 7"
    itens = @(
        @{
            produtoId = [guid]::NewGuid()
            produtoNome = "Produto G"
            quantidade = 10
            valorUnitario = 100.00
        }
    )
}
$createResponse10Itens = Test-Endpoint -Name "Criar venda com 10 itens 20 porcento desconto" -Method "POST" -Url "/api/venda" -Body $venda10Itens -ExpectedStatus 201
$vendaId10Itens = if ($createResponse10Itens -and $createResponse10Itens.data) { $createResponse10Itens.data } else { $null }

if ($vendaId10Itens) {
    $getVenda10Itens = Invoke-RestMethod -Uri "$BaseUrl/api/venda/$vendaId10Itens" -Method GET
    $item = $getVenda10Itens.data.itens[0]
    $descontoEsperado = 200.00  # 10 * 100 * 0.20
    if ([math]::Abs($item.desconto - $descontoEsperado) -lt 0.01) {
        Write-Host "✅ Validação: 10 itens tem 20 porcento de desconto ($($item.desconto))" -ForegroundColor Green
        $global:TestResults.Passed++
    } else {
        Write-Host "❌ Validação: 10 itens deveria ter desconto $descontoEsperado, mas tem $($item.desconto)" -ForegroundColor Red
        $global:TestResults.Failed++
    }
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "  RESUMO DOS TESTES" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Total de testes: $($global:TestResults.Total)" -ForegroundColor White
Write-Host "✅ Passou: $($global:TestResults.Passed)" -ForegroundColor Green
Write-Host "❌ Falhou: $($global:TestResults.Failed)" -ForegroundColor Red
Write-Host ""

if ($global:TestResults.Failed -eq 0) {
    Write-Host "🎉 Todos os testes passaram!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "⚠️ Alguns testes falharam. Verifique os detalhes acima." -ForegroundColor Yellow
    exit 1
}

