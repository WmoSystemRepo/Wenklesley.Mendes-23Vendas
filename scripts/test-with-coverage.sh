#!/bin/bash
# Script para executar testes com cobertura
# Gera relatórios em múltiplos formatos: OpenCover, Cobertura e JSON

CONFIGURATION=${1:-Debug}
VERBOSE=${2:-false}

echo "Executando testes com cobertura..."

# Navegar para o diretório do projeto de testes
TEST_PROJECT_PATH="tests/UnitTests/UnitTests.csproj"
if [ ! -f "$TEST_PROJECT_PATH" ]; then
    echo "ERRO: Projeto de testes não encontrado em $TEST_PROJECT_PATH"
    exit 1
fi

# Criar diretório de resultados se não existir
RESULTS_DIR="tests/UnitTests/TestResults"
mkdir -p "$RESULTS_DIR"

# Executar testes com cobertura
echo ""
echo "Executando testes..."

VERBOSITY="minimal"
if [ "$VERBOSE" = "true" ]; then
    VERBOSITY="detailed"
fi

dotnet test "$TEST_PROJECT_PATH" \
    --configuration "$CONFIGURATION" \
    /p:CollectCoverage=true \
    /p:CoverletOutputFormat=opencover\;cobertura\;json \
    /p:CoverletOutput="$RESULTS_DIR/coverage" \
    /p:ExcludeByAttribute=Obsolete,GeneratedCodeAttribute,CompilerGeneratedAttribute \
    /p:Exclude="[*.Tests]*,[*.Test]*" \
    /p:IncludeDirectory=../../src \
    --verbosity "$VERBOSITY"

if [ $? -ne 0 ]; then
    echo ""
    echo "ERRO: Testes falharam"
    exit 1
fi

echo ""
echo "Testes executados com sucesso!"
echo ""
echo "Relatórios de cobertura gerados em:"
echo "  - OpenCover: $RESULTS_DIR/coverage.opencover.xml"
echo "  - Cobertura: $RESULTS_DIR/coverage.cobertura.xml"
echo "  - JSON: $RESULTS_DIR/coverage.json"

# Tentar exibir resumo se possível
JSON_PATH="$RESULTS_DIR/coverage.json"
if [ -f "$JSON_PATH" ] && command -v jq &> /dev/null; then
    echo ""
    echo "Resumo de Cobertura:"
    LINE_COV=$(jq -r '.summary.linecoverage * 100' "$JSON_PATH" 2>/dev/null)
    BRANCH_COV=$(jq -r '.summary.branchcoverage * 100' "$JSON_PATH" 2>/dev/null)
    METHOD_COV=$(jq -r '.summary.methodcoverage * 100' "$JSON_PATH" 2>/dev/null)
    
    if [ "$LINE_COV" != "null" ]; then
        printf "  - Linhas: %.2f%%\n" "$LINE_COV"
        printf "  - Branches: %.2f%%\n" "$BRANCH_COV"
        printf "  - Métodos: %.2f%%\n" "$METHOD_COV"
    fi
fi

echo ""
echo "Para visualizar relatórios HTML, use ferramentas como:"
echo "  - ReportGenerator: dotnet tool install -g dotnet-reportgenerator-globaltool"
echo "  - Depois execute: reportgenerator -reports:$RESULTS_DIR/coverage.opencover.xml -targetdir:$RESULTS_DIR/html -reporttypes:Html"

