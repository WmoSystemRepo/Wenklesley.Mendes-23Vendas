# Script para gerar relatório Markdown a partir do relatório JSON
# Inclui todos os JSONs de request e response como evidências

param(
    [string]$JsonPath,
    [string]$OutputPath
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path $JsonPath)) {
    Write-Host "❌ Arquivo JSON não encontrado: $JsonPath" -ForegroundColor Red
    exit 1
}

# Carregar relatório JSON
try {
    $jsonContent = Get-Content $JsonPath -Raw -Encoding UTF8
    $report = $jsonContent | ConvertFrom-Json
} catch {
    Write-Host "❌ Erro ao carregar arquivo JSON: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Função para formatar JSON de forma legível
function Format-JsonForMarkdown {
    param([string]$JsonString)
    
    if ([string]::IsNullOrWhiteSpace($JsonString)) {
        $result = [Environment]::NewLine
        $result += '```json'
        $result += [Environment]::NewLine
        $result += 'null'
        $result += [Environment]::NewLine
        $result += '```'
        return $result
    }
    
    try {
        $obj = $JsonString | ConvertFrom-Json
        $formatted = $obj | ConvertTo-Json -Depth 10
        $result = [Environment]::NewLine
        $result += '```json'
        $result += [Environment]::NewLine
        $result += $formatted
        $result += [Environment]::NewLine
        $result += '```'
        return $result
    } catch {
        $result = [Environment]::NewLine
        $result += '```json'
        $result += [Environment]::NewLine
        $result += $JsonString
        $result += [Environment]::NewLine
        $result += '```'
        return $result
    }
}

# Função para formatar headers
function Format-Headers {
    param([object]$Headers)
    
    if ($null -eq $Headers) {
        return "Nenhum header"
    }
    
    if ($Headers -is [hashtable] -and $Headers.Count -eq 0) {
        return "Nenhum header"
    }
    
    $result = ""
    if ($Headers -is [hashtable]) {
        foreach ($key in $Headers.Keys) {
            $result += "- "
            $result += "**"
            $result += $key
            $result += "**: "
            $result += $Headers[$key]
            $result += [Environment]::NewLine
        }
    } elseif ($Headers -is [PSCustomObject]) {
        $Headers.PSObject.Properties | ForEach-Object {
            $result += "- "
            $result += "**"
            $result += $_.Name
            $result += "**: "
            $result += $_.Value
            $result += [Environment]::NewLine
        }
    }
    return $result.TrimEnd()
}

# Iniciar geração do Markdown
$md = "# Relatório de Testes - API 123Vendas`n`n"
$md += "## Informações da Execução`n`n"
$md += "- **Data de Início**: $($report.testRun.startTime)`n"
$md += "- **Data de Término**: $($report.testRun.endTime)`n"
$md += "- **Duração**: $($report.testRun.duration)`n"
$md += "- **URL Base**: $($report.testRun.baseUrl)`n"
$md += "- **Total de Testes**: $($report.testRun.totalTests)`n"
$md += "- **Passou**: $($report.testRun.passed)`n"
$md += "- **Falhou**: $($report.testRun.failed)`n"
$md += "- **Taxa de Sucesso**: $($report.testRun.successRate)%`n`n"
$md += "## Resumo Executivo`n`n"
$md += "Este relatório contém os resultados de $($report.testRun.totalTests) testes executados na API 123Vendas.`n`n"
$md += "**Resultados Gerais:**`n"
$md += "- Testes que passaram: **$($report.testRun.passed)**`n"
$md += "- Testes que falharam: **$($report.testRun.failed)**`n"
$md += "- Taxa de sucesso: **$($report.testRun.successRate)%**`n`n"
$md += "## Resumo por Categoria`n`n"

# Adicionar resumo por categoria
if ($report.summary -and $report.summary.byCategory) {
    $byCategory = $report.summary.byCategory
    if ($byCategory.PSObject.Properties) {
        foreach ($prop in $byCategory.PSObject.Properties) {
            $category = $prop.Name
            $catData = $prop.Value
            $md += "`n### $category`n"
            $md += "- Total: $($catData.total)`n"
            $md += "- Passou: $($catData.passed)`n"
            $md += "- Falhou: $($catData.failed)`n"
        }
    }
}

$md += "`n`n## Resumo por Status HTTP`n`n"

# Adicionar resumo por status HTTP
if ($report.summary -and $report.summary.byStatus) {
    $byStatus = $report.summary.byStatus
    if ($byStatus.PSObject.Properties) {
        $statusList = @()
        foreach ($prop in $byStatus.PSObject.Properties) {
            $statusList += [PSCustomObject]@{ Name = $prop.Name; Count = $prop.Value }
        }
        foreach ($status in ($statusList | Sort-Object Name)) {
            $md += "- **$($status.Name)**: $($status.Count)`n"
        }
    }
}

# Agrupar testes por categoria
$testsByCategory = @{}
if ($report.tests) {
    foreach ($test in $report.tests) {
        $category = if ($test.category) { $test.category } else { "SemCategoria" }
        if (-not $testsByCategory.ContainsKey($category)) {
            $testsByCategory[$category] = @()
        }
        $testsByCategory[$category] += $test
    }
}

# Gerar seções por categoria
$md += "`n`n---`n`n## Testes por Categoria`n`n"

foreach ($category in $testsByCategory.Keys | Sort-Object) {
    $md += "### $category`n`n"
    
    foreach ($test in $testsByCategory[$category]) {
        $statusIcon = if ($test.status -eq "PASSED") { "[OK]" } else { "[FAIL]" }
        
        $md += "#### Teste: $($test.name)`n`n"
        $md += "- **Status**: $statusIcon $($test.status)`n"
        $md += "- **ID**: $($test.id)`n"
        $md += "- **Endpoint**: $($test.endpoint)`n"
        $md += "- **Duração**: $($test.duration)ms`n"
        $md += "- **Timestamp**: $($test.timestamp)`n`n"
        $md += "**Request**:`n`n"
        
        # Request
        $requestJson = @{
            method = $test.request.method
            url = $test.request.url
            headers = $test.request.headers
            body = if ($test.request.body) { ($test.request.body | ConvertFrom-Json) } else { $null }
        } | ConvertTo-Json -Depth 10
        
        $md += Format-JsonForMarkdown -JsonString $requestJson
        
        # Response
        $md += "`n`n**Response** (Status: $($test.actualStatus)):"
        
        if ($test.response.body) {
            $md += Format-JsonForMarkdown -JsonString $test.response.body
        } else {
            $md += [Environment]::NewLine
            $md += '```json'
            $md += [Environment]::NewLine
            $md += 'null'
            $md += [Environment]::NewLine
            $md += '```'
        }
        
        # Headers da resposta
        $md += "`n`n"
        $md += "**Headers da Resposta**:`n`n"
        $md += Format-Headers -Headers $test.response.headers
        
        # Validações
        if ($test.validations.Count -gt 0) {
            $md += "`n`n**Validações**:`n`n"
            foreach ($validation in $test.validations) {
                $validationIcon = if ($validation.passed) { "[OK]" } else { "[FAIL]" }
                $md += "- $validationIcon **$($validation.name)**: $($validation.message)`n"
            }
        }
        
        # Erro (se houver)
        if ($test.error) {
            $md += [Environment]::NewLine
            $md += [Environment]::NewLine
            $md += '**Erro**:'
            $md += [Environment]::NewLine
            $md += [Environment]::NewLine
            $md += '```'
            $md += [Environment]::NewLine
            $md += $test.error
            $md += [Environment]::NewLine
            $md += '```'
        }
        
        $md += [Environment]::NewLine
        $md += [Environment]::NewLine
        $md += '---'
        $md += [Environment]::NewLine
        $md += [Environment]::NewLine
    }
}

# Seção de análise de falhas
$failedTests = @()
if ($report.tests) {
    $failedTests = $report.tests | Where-Object { $_.status -eq "FAILED" }
}
if ($failedTests -and $failedTests.Count -gt 0) {
    $md += "## Análise de Falhas`n`n"
    $md += "Total de testes que falharam: **$($failedTests.Count)**`n`n"
    
    foreach ($test in $failedTests) {
        $md += "### [FAIL] $($test.name)`n`n"
        $md += "- **Categoria**: $($test.category)`n"
        $md += "- **Endpoint**: $($test.endpoint)`n"
        $md += "- **Status Esperado**: $($test.expectedStatus)`n"
        $md += "- **Status Recebido**: $($test.actualStatus)`n"
        $md += "- **Erro**: $($test.error)`n`n"
        $md += "**Request**:`n`n"
        
        $requestJson = @{
            method = $test.request.method
            url = $test.request.url
            headers = $test.request.headers
            body = if ($test.request.body) { ($test.request.body | ConvertFrom-Json) } else { $null }
        } | ConvertTo-Json -Depth 10
        
        $md += Format-JsonForMarkdown -JsonString $requestJson
        
        $md += "`n`n**Response**:"
        
        if ($test.response.body) {
            $md += Format-JsonForMarkdown -JsonString $test.response.body
        } else {
            $md += [Environment]::NewLine
            $md += '```json'
            $md += [Environment]::NewLine
            $md += 'null'
            $md += [Environment]::NewLine
            $md += '```'
        }
        
        $md += [Environment]::NewLine
        $md += [Environment]::NewLine
        $md += '---'
        $md += [Environment]::NewLine
        $md += [Environment]::NewLine
    }
}

# Rodapé
$md += "`n`n---`n`n"
$md += "## Conclusão`n`n"
$dateStr = Get-Date -Format 'yyyy-MM-dd HH:mm:ss'
$md += "Este relatório foi gerado automaticamente em $dateStr."
$md += [Environment]::NewLine
$md += [Environment]::NewLine
$jsonFileName = Split-Path $JsonPath -Leaf
$md += "Para mais informações sobre os testes, consulte o arquivo JSON completo: `"$jsonFileName`""
$md += [Environment]::NewLine
$md += [Environment]::NewLine
$md += '---'
$md += [Environment]::NewLine
$md += [Environment]::NewLine
$md += '**Gerado por**: Script de Testes Automatizado - API 123Vendas'
$md += [Environment]::NewLine
$md += '**Versão**: 1.0.0'
$md += [Environment]::NewLine
$md += [Environment]::NewLine

# Salvar arquivo Markdown
$md | Out-File -FilePath $OutputPath -Encoding UTF8

Write-Host "[OK] Relatorio Markdown gerado: $OutputPath" -ForegroundColor Green

