# Script completo de testes da API 123Vendas
# Captura todas as respostas HTTP e gera relatório JSON detalhado

param(
    [string]$BaseUrl = "http://localhost:5000"
)

$ErrorActionPreference = "Continue"

# Estrutura de dados para relatório
$script:TestRun = @{
    startTime = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
    endTime = $null
    duration = $null
    baseUrl = $BaseUrl
    totalTests = 0
    passed = 0
    failed = 0
    successRate = 0.0
}

$script:Tests = @()
$script:TestCounter = 0

# Função para executar teste e capturar resposta completa
function Invoke-Test {
    param(
        [string]$Id,
        [string]$Name,
        [string]$Category,
        [string]$Method,
        [string]$Url,
        [object]$Body = $null,
        [int]$ExpectedStatus = 200,
        [scriptblock]$Validation = $null,
        [hashtable]$CustomHeaders = @{}
    )
    
    $script:TestCounter++
    $script:TestRun.totalTests++
    
    $testStartTime = Get-Date
    $test = @{
        id = $Id
        name = $Name
        category = $Category
        endpoint = "$Method $Url"
        status = "PENDING"
        expectedStatus = $ExpectedStatus
        actualStatus = $null
        request = @{
            method = $Method
            url = $Url
            headers = @{}
            body = $Body
        }
        response = @{
            statusCode = $null
            headers = @{}
            body = $null
        }
        validations = @()
        duration = 0
        timestamp = $testStartTime.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        error = $null
    }
    
    # Preparar headers
    $headers = @{
        "Content-Type" = "application/json"
        "Accept" = "application/json"
    }
    foreach ($key in $CustomHeaders.Keys) {
        $headers[$key] = $CustomHeaders[$key]
    }
    $test.request.headers = $headers
    
    try {
        $params = @{
            Uri = "$BaseUrl$Url"
            Method = $Method
            Headers = $headers
            ErrorAction = "Stop"
        }
        
        if ($Body) {
            $jsonBody = $Body | ConvertTo-Json -Depth 10 -Compress
            $params.Body = $jsonBody
            $test.request.body = ($Body | ConvertTo-Json -Depth 10)
        } else {
            $test.request.body = $null
        }
        
        # Executar requisição e capturar resposta completa
        try {
            $response = Invoke-WebRequest @params -UseBasicParsing
            $test.actualStatus = $response.StatusCode
            $test.response.statusCode = $response.StatusCode
            
            # Capturar headers
            foreach ($headerName in $response.Headers.Keys) {
                $test.response.headers[$headerName] = $response.Headers[$headerName]
            }
            
            # Capturar body
            try {
                $test.response.body = $response.Content | ConvertFrom-Json | ConvertTo-Json -Depth 10
            } catch {
                $test.response.body = $response.Content
            }
            
            # Validar status code
            $statusValidation = @{
                name = "Status Code"
                passed = ($response.StatusCode -eq $ExpectedStatus)
                message = if ($response.StatusCode -eq $ExpectedStatus) { "Status code correto" } else { "Esperado: $ExpectedStatus, Recebido: $response.StatusCode" }
            }
            $test.validations += $statusValidation
            
            if ($response.StatusCode -ne $ExpectedStatus) {
                throw "Status code esperado: $ExpectedStatus, recebido: $response.StatusCode"
            }
            
            # Executar validações customizadas
            if ($Validation) {
                try {
                    $responseObj = $response.Content | ConvertFrom-Json
                    $validationResult = & $Validation $responseObj
                    $customValidation = @{
                        name = "Validação Customizada"
                        passed = $validationResult
                        message = if ($validationResult) { "Validação passou" } else { "Validação falhou" }
                    }
                    $test.validations += $customValidation
                    
                    if (-not $validationResult) {
                        throw "Validação customizada falhou"
                    }
                } catch {
                    $customValidation = @{
                        name = "Validação Customizada"
                        passed = $false
                        message = $_.Exception.Message
                    }
                    $test.validations += $customValidation
                    throw
                }
            }
            
            $test.status = "PASSED"
            $script:TestRun.passed++
            Write-Host "✅ $Name" -ForegroundColor Green
            
        } catch {
            # Capturar erro de resposta HTTP
            if ($_.Exception.Response) {
                $statusCode = [int]$_.Exception.Response.StatusCode
                $test.actualStatus = $statusCode
                $test.response.statusCode = $statusCode
                
                # Capturar headers de erro
                foreach ($headerName in $_.Exception.Response.Headers.Keys) {
                    $test.response.headers[$headerName] = $_.Exception.Response.Headers[$headerName]
                }
                
                # Capturar body de erro
                try {
                    $reader = New-Object System.IO.StreamReader($_.Exception.Response.GetResponseStream())
                    $errorBody = $reader.ReadToEnd()
                    $reader.Close()
                    try {
                        $test.response.body = $errorBody | ConvertFrom-Json | ConvertTo-Json -Depth 10
                    } catch {
                        $test.response.body = $errorBody
                    }
                } catch {
                    $test.response.body = $null
                }
                
                # Validar se o status de erro era esperado
                if ($statusCode -eq $ExpectedStatus) {
                    $test.status = "PASSED"
                    $script:TestRun.passed++
                    $statusValidation = @{
                        name = "Status Code (Erro Esperado)"
                        passed = $true
                        message = "Status code de erro esperado: $statusCode"
                    }
                    $test.validations += $statusValidation
                    Write-Host "✅ $Name (erro esperado: $statusCode)" -ForegroundColor Green
                } else {
                    $test.status = "FAILED"
                    $script:TestRun.failed++
                    $test.error = $_.Exception.Message
                    Write-Host "❌ $Name" -ForegroundColor Red
                    Write-Host "   Erro: $($_.Exception.Message)" -ForegroundColor Yellow
                }
            } else {
                throw
            }
        }
        
    } catch {
        $test.status = "FAILED"
        $script:TestRun.failed++
        $test.error = $_.Exception.Message
        Write-Host "❌ $Name" -ForegroundColor Red
        Write-Host "   Erro: $($_.Exception.Message)" -ForegroundColor Yellow
    }
    
    $testEndTime = Get-Date
    $test.duration = [math]::Round(($testEndTime - $testStartTime).TotalMilliseconds, 2)
    
    $script:Tests += $test
    return $test
}

# Iniciar testes
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  TESTE COMPLETO DA API 123VENDAS" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Variáveis para armazenar IDs criados durante os testes
$script:CreatedVendaIds = @{}

# ============================================
# CATEGORIA: Health
# ============================================
Write-Host "CATEGORIA: Health Check" -ForegroundColor Yellow
Invoke-Test -Id "test-001" -Name "Health Check" -Category "Health" -Method "GET" -Url "/health" -ExpectedStatus 200 -Validation {
    param($response)
    $response.status -eq "healthy"
}

# ============================================
# CATEGORIA: Vendas - Listagem
# ============================================
Write-Host "`nCATEGORIA: Vendas - Listagem" -ForegroundColor Yellow
Invoke-Test -Id "test-002" -Name "Listar Vendas (pode estar vazio)" -Category "Vendas" -Method "GET" -Url "/api/venda" -ExpectedStatus 200 -Validation {
    param($response)
    $response.success -eq $true -and $null -ne $response.data
}

# ============================================
# CATEGORIA: Vendas - Criação
# ============================================
Write-Host "`nCATEGORIA: Vendas - Criação" -ForegroundColor Yellow

# Teste 1: Criar venda com 3 itens (sem desconto)
$venda1 = @{
    numeroVenda = "V001"
    clienteId = [guid]::NewGuid().ToString()
    clienteNome = "Cliente Teste 1"
    filialId = [guid]::NewGuid().ToString()
    filialNome = "Filial Teste 1"
    itens = @(
        @{
            produtoId = [guid]::NewGuid().ToString()
            produtoNome = "Produto A"
            quantidade = 3
            valorUnitario = 100.00
        }
    )
}
$test1 = Invoke-Test -Id "test-003" -Name "Criar venda com 3 itens (sem desconto)" -Category "Vendas" -Method "POST" -Url "/api/venda" -Body $venda1 -ExpectedStatus 201 -Validation {
    param($response)
    $response.success -eq $true -and $response.data -ne $null -and $response.data -ne [guid]::Empty.ToString()
}
if ($test1.status -eq "PASSED" -and $test1.response.body) {
    $responseObj = $test1.response.body | ConvertFrom-Json
    if ($responseObj.data) {
        $script:CreatedVendaIds["V001"] = $responseObj.data
    }
}

# Teste 2: Criar venda com 5 itens (10% desconto)
$venda2 = @{
    numeroVenda = "V002"
    clienteId = [guid]::NewGuid().ToString()
    clienteNome = "Cliente Teste 2"
    filialId = [guid]::NewGuid().ToString()
    filialNome = "Filial Teste 2"
    itens = @(
        @{
            produtoId = [guid]::NewGuid().ToString()
            produtoNome = "Produto B"
            quantidade = 5
            valorUnitario = 100.00
        }
    )
}
$test2 = Invoke-Test -Id "test-004" -Name "Criar venda com 5 itens (10% desconto)" -Category "Vendas" -Method "POST" -Url "/api/venda" -Body $venda2 -ExpectedStatus 201
if ($test2.status -eq "PASSED" -and $test2.response.body) {
    $responseObj = $test2.response.body | ConvertFrom-Json
    if ($responseObj.data) {
        $script:CreatedVendaIds["V002"] = $responseObj.data
    }
}

# Teste 3: Criar venda com 15 itens (20% desconto)
$venda3 = @{
    numeroVenda = "V003"
    clienteId = [guid]::NewGuid().ToString()
    clienteNome = "Cliente Teste 3"
    filialId = [guid]::NewGuid().ToString()
    filialNome = "Filial Teste 3"
    itens = @(
        @{
            produtoId = [guid]::NewGuid().ToString()
            produtoNome = "Produto C"
            quantidade = 15
            valorUnitario = 100.00
        }
    )
}
$test3 = Invoke-Test -Id "test-005" -Name "Criar venda com 15 itens (20% desconto)" -Category "Vendas" -Method "POST" -Url "/api/venda" -Body $venda3 -ExpectedStatus 201
if ($test3.status -eq "PASSED" -and $test3.response.body) {
    $responseObj = $test3.response.body | ConvertFrom-Json
    if ($responseObj.data) {
        $script:CreatedVendaIds["V003"] = $responseObj.data
    }
}

# Teste 4: Criar venda com 21 itens (deve falhar)
$venda4 = @{
    numeroVenda = "V004"
    clienteId = [guid]::NewGuid().ToString()
    clienteNome = "Cliente Teste 4"
    filialId = [guid]::NewGuid().ToString()
    filialNome = "Filial Teste 4"
    itens = @(
        @{
            produtoId = [guid]::NewGuid().ToString()
            produtoNome = "Produto D"
            quantidade = 21
            valorUnitario = 100.00
        }
    )
}
Invoke-Test -Id "test-006" -Name "Criar venda com 21 itens (deve falhar)" -Category "RegrasNegocio" -Method "POST" -Url "/api/venda" -Body $venda4 -ExpectedStatus 400

# Teste 5: Criar venda com múltiplos itens
$venda5 = @{
    numeroVenda = "V005"
    clienteId = [guid]::NewGuid().ToString()
    clienteNome = "Cliente Teste 5"
    filialId = [guid]::NewGuid().ToString()
    filialNome = "Filial Teste 5"
    itens = @(
        @{
            produtoId = [guid]::NewGuid().ToString()
            produtoNome = "Produto E1"
            quantidade = 3
            valorUnitario = 50.00
        },
        @{
            produtoId = [guid]::NewGuid().ToString()
            produtoNome = "Produto E2"
            quantidade = 5
            valorUnitario = 100.00
        },
        @{
            produtoId = [guid]::NewGuid().ToString()
            produtoNome = "Produto E3"
            quantidade = 10
            valorUnitario = 200.00
        }
    )
}
$test5 = Invoke-Test -Id "test-007" -Name "Criar venda com múltiplos itens" -Category "Vendas" -Method "POST" -Url "/api/venda" -Body $venda5 -ExpectedStatus 201
if ($test5.status -eq "PASSED" -and $test5.response.body) {
    $responseObj = $test5.response.body | ConvertFrom-Json
    if ($responseObj.data) {
        $script:CreatedVendaIds["V005"] = $responseObj.data
    }
}

# ============================================
# CATEGORIA: Vendas - Busca
# ============================================
Write-Host "`nCATEGORIA: Vendas - Busca" -ForegroundColor Yellow

if ($script:CreatedVendaIds.ContainsKey("V001")) {
    Invoke-Test -Id "test-008" -Name "Obter venda por ID (existente)" -Category "Vendas" -Method "GET" -Url "/api/venda/$($script:CreatedVendaIds['V001'])" -ExpectedStatus 200 -Validation {
        param($response)
        $response.success -eq $true -and $response.data -ne $null -and $response.data.numeroVenda -eq "V001"
    }
}

Invoke-Test -Id "test-009" -Name "Obter venda por ID (inexistente)" -Category "Vendas" -Method "GET" -Url "/api/venda/00000000-0000-0000-0000-000000000000" -ExpectedStatus 404

# ============================================
# CATEGORIA: Regras de Negócio - Descontos
# ============================================
Write-Host "`nCATEGORIA: Regras de Negócio - Descontos" -ForegroundColor Yellow

if ($script:CreatedVendaIds.ContainsKey("V002")) {
    Invoke-Test -Id "test-010" -Name "Verificar desconto 10% (5 itens)" -Category "RegrasNegocio" -Method "GET" -Url "/api/venda/$($script:CreatedVendaIds['V002'])" -ExpectedStatus 200 -Validation {
        param($response)
        if ($response.success -and $response.data -and $response.data.itens.Count -gt 0) {
            $item = $response.data.itens[0]
            $descontoEsperado = 50.00  # 5 * 100 * 0.10
            $valorTotalEsperado = 450.00  # 500 - 50
            return ([math]::Abs($item.desconto - $descontoEsperado) -lt 0.01) -and 
                   ([math]::Abs($item.valorTotalItem - $valorTotalEsperado) -lt 0.01)
        }
        return $false
    }
}

if ($script:CreatedVendaIds.ContainsKey("V003")) {
    Invoke-Test -Id "test-011" -Name "Verificar desconto 20% (15 itens)" -Category "RegrasNegocio" -Method "GET" -Url "/api/venda/$($script:CreatedVendaIds['V003'])" -ExpectedStatus 200 -Validation {
        param($response)
        if ($response.success -and $response.data -and $response.data.itens.Count -gt 0) {
            $item = $response.data.itens[0]
            $descontoEsperado = 300.00  # 15 * 100 * 0.20
            $valorTotalEsperado = 1200.00  # 1500 - 300
            return ([math]::Abs($item.desconto - $descontoEsperado) -lt 0.01) -and 
                   ([math]::Abs($item.valorTotalItem - $valorTotalEsperado) -lt 0.01)
        }
        return $false
    }
}

# Teste com 4 itens (sem desconto)
$venda4Itens = @{
    numeroVenda = "V006"
    clienteId = [guid]::NewGuid().ToString()
    clienteNome = "Cliente Teste 6"
    filialId = [guid]::NewGuid().ToString()
    filialNome = "Filial Teste 6"
    itens = @(
        @{
            produtoId = [guid]::NewGuid().ToString()
            produtoNome = "Produto F"
            quantidade = 4
            valorUnitario = 100.00
        }
    )
}
$test4Itens = Invoke-Test -Id "test-012" -Name "Criar venda com 4 itens (sem desconto)" -Category "RegrasNegocio" -Method "POST" -Url "/api/venda" -Body $venda4Itens -ExpectedStatus 201
if ($test4Itens.status -eq "PASSED" -and $test4Itens.response.body) {
    $responseObj = $test4Itens.response.body | ConvertFrom-Json
    if ($responseObj.data) {
        $vendaId4Itens = $responseObj.data
        Invoke-Test -Id "test-013" -Name "Verificar sem desconto (4 itens)" -Category "RegrasNegocio" -Method "GET" -Url "/api/venda/$vendaId4Itens" -ExpectedStatus 200 -Validation {
            param($response)
            if ($response.success -and $response.data -and $response.data.itens.Count -gt 0) {
                $item = $response.data.itens[0]
                return $item.desconto -eq 0
            }
            return $false
        }
    }
}

# ============================================
# CATEGORIA: Vendas - Atualização
# ============================================
Write-Host "`nCATEGORIA: Vendas - Atualização" -ForegroundColor Yellow

if ($script:CreatedVendaIds.ContainsKey("V001")) {
    # Adicionar item
    $update1 = @{
        itensParaAdicionar = @(
            @{
                produtoId = [guid]::NewGuid().ToString()
                produtoNome = "Produto Novo"
                quantidade = 5
                valorUnitario = 50.00
            }
        )
    }
    Invoke-Test -Id "test-014" -Name "Adicionar item à venda" -Category "Vendas" -Method "PUT" -Url "/api/venda/$($script:CreatedVendaIds['V001'])" -Body $update1 -ExpectedStatus 204
}

if ($script:CreatedVendaIds.ContainsKey("V002")) {
    # Buscar venda para obter item ID
    try {
        $getVenda2 = Invoke-RestMethod -Uri "$BaseUrl/api/venda/$($script:CreatedVendaIds['V002'])" -Method GET
        if ($getVenda2.data -and $getVenda2.data.itens.Count -gt 0) {
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
            Invoke-Test -Id "test-015" -Name "Atualizar quantidade de item (5 para 10, desconto muda para 20%)" -Category "Vendas" -Method "PUT" -Url "/api/venda/$($script:CreatedVendaIds['V002'])" -Body $update2 -ExpectedStatus 204
        }
    } catch {
        Write-Host "⚠️  Não foi possível obter item para atualização" -ForegroundColor Yellow
    }
}

# ============================================
# CATEGORIA: Vendas - Cancelamento
# ============================================
Write-Host "`nCATEGORIA: Vendas - Cancelamento" -ForegroundColor Yellow

# Criar uma venda para cancelar
$vendaParaCancelar = @{
    numeroVenda = "V007"
    clienteId = [guid]::NewGuid().ToString()
    clienteNome = "Cliente Teste 7"
    filialId = [guid]::NewGuid().ToString()
    filialNome = "Filial Teste 7"
    itens = @(
        @{
            produtoId = [guid]::NewGuid().ToString()
            produtoNome = "Produto G"
            quantidade = 3
            valorUnitario = 100.00
        }
    )
}
$testCancelar = Invoke-Test -Id "test-016" -Name "Criar venda para cancelar" -Category "Vendas" -Method "POST" -Url "/api/venda" -Body $vendaParaCancelar -ExpectedStatus 201
$vendaIdParaCancelar = $null
if ($testCancelar.status -eq "PASSED" -and $testCancelar.response.body) {
    $responseObj = $testCancelar.response.body | ConvertFrom-Json
    if ($responseObj.data) {
        $vendaIdParaCancelar = $responseObj.data
    }
}

if ($vendaIdParaCancelar) {
    Invoke-Test -Id "test-017" -Name "Cancelar venda" -Category "Vendas" -Method "DELETE" -Url "/api/venda/$vendaIdParaCancelar" -ExpectedStatus 204
    
    # Tentar adicionar item a venda cancelada
    $updateCancelada = @{
        itensParaAdicionar = @(
            @{
                produtoId = [guid]::NewGuid().ToString()
                produtoNome = "Produto Teste"
                quantidade = 1
                valorUnitario = 10.00
            }
        )
    }
    Invoke-Test -Id "test-018" -Name "Tentar adicionar item a venda cancelada (deve falhar)" -Category "Vendas" -Method "PUT" -Url "/api/venda/$vendaIdParaCancelar" -Body $updateCancelada -ExpectedStatus 400
}

Invoke-Test -Id "test-019" -Name "Cancelar venda inexistente" -Category "Vendas" -Method "DELETE" -Url "/api/venda/00000000-0000-0000-0000-000000000000" -ExpectedStatus 404

# ============================================
# CATEGORIA: Validações
# ============================================
Write-Host "`nCATEGORIA: Validações" -ForegroundColor Yellow

# Venda sem número
$vendaSemNumero = @{
    clienteId = [guid]::NewGuid().ToString()
    clienteNome = "Cliente Teste"
    filialId = [guid]::NewGuid().ToString()
    filialNome = "Filial Teste"
    itens = @(
        @{
            produtoId = [guid]::NewGuid().ToString()
            produtoNome = "Produto"
            quantidade = 3
            valorUnitario = 100.00
        }
    )
}
Invoke-Test -Id "test-020" -Name "Criar venda sem número (deve falhar)" -Category "Validacoes" -Method "POST" -Url "/api/venda" -Body $vendaSemNumero -ExpectedStatus 400

# Venda com quantidade zero
$vendaQtdZero = @{
    numeroVenda = "V008"
    clienteId = [guid]::NewGuid().ToString()
    clienteNome = "Cliente Teste"
    filialId = [guid]::NewGuid().ToString()
    filialNome = "Filial Teste"
    itens = @(
        @{
            produtoId = [guid]::NewGuid().ToString()
            produtoNome = "Produto"
            quantidade = 0
            valorUnitario = 100.00
        }
    )
}
Invoke-Test -Id "test-021" -Name "Criar venda com quantidade zero (deve falhar)" -Category "Validacoes" -Method "POST" -Url "/api/venda" -Body $vendaQtdZero -ExpectedStatus 400

# ============================================
# Finalizar e gerar relatório
# ============================================
$script:TestRun.endTime = (Get-Date).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
$startTime = [DateTime]::Parse($script:TestRun.startTime)
$endTime = [DateTime]::Parse($script:TestRun.endTime)
$duration = $endTime - $startTime
$script:TestRun.duration = "{0:hh\:mm\:ss}" -f $duration

if ($script:TestRun.totalTests -gt 0) {
    $script:TestRun.successRate = [math]::Round(($script:TestRun.passed / $script:TestRun.totalTests) * 100, 2)
}

# Calcular resumo por categoria
$summaryByCategory = @{}
$summaryByStatus = @{}

foreach ($test in $script:Tests) {
    # Por categoria
    if (-not $summaryByCategory.ContainsKey($test.category)) {
        $summaryByCategory[$test.category] = @{
            total = 0
            passed = 0
            failed = 0
        }
    }
    $summaryByCategory[$test.category].total++
    if ($test.status -eq "PASSED") {
        $summaryByCategory[$test.category].passed++
    } else {
        $summaryByCategory[$test.category].failed++
    }
    
    # Por status HTTP
    if ($test.actualStatus) {
        $statusKey = $test.actualStatus.ToString()
        if (-not $summaryByStatus.ContainsKey($statusKey)) {
            $summaryByStatus[$statusKey] = 0
        }
        $summaryByStatus[$statusKey]++
    }
}

# Montar relatório final
$report = @{
    testRun = $script:TestRun
    tests = $script:Tests
    summary = @{
        byCategory = $summaryByCategory
        byStatus = $summaryByStatus
    }
}

# Exibir resumo
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  RESUMO DOS TESTES" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Total de testes: $($script:TestRun.totalTests)" -ForegroundColor White
Write-Host "✅ Passou: $($script:TestRun.passed)" -ForegroundColor Green
Write-Host "❌ Falhou: $($script:TestRun.failed)" -ForegroundColor Red
Write-Host "Taxa de sucesso: $($script:TestRun.successRate)%" -ForegroundColor $(if ($script:TestRun.successRate -ge 90) { "Green" } else { "Yellow" })
Write-Host "Duração: $($script:TestRun.duration)" -ForegroundColor Gray
Write-Host ""

# Retornar relatório
return $report

