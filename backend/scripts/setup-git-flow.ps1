# Script para configurar Git Flow no repositório
# Execute este script após inicializar o repositório Git

Write-Host "Configurando Git Flow..." -ForegroundColor Cyan

# Verificar se Git está instalado
if (-not (Get-Command git -ErrorAction SilentlyContinue)) {
    Write-Host "ERRO: Git não está instalado ou não está no PATH" -ForegroundColor Red
    exit 1
}

# Verificar se estamos em um repositório Git
if (-not (Test-Path .git)) {
    Write-Host "Inicializando repositório Git..." -ForegroundColor Yellow
    git init
    git branch -M main
}

# Configurar template de commit
if (Test-Path .gitmessage) {
    git config commit.template .gitmessage
    Write-Host "Template de commit configurado" -ForegroundColor Green
}

# Configurar remote (se ainda não estiver configurado)
$remoteUrl = "https://github.com/WmoSystemRepo/Teste_Admiss-o_Backend.git"
$currentRemote = git remote get-url origin 2>$null

if ($LASTEXITCODE -ne 0) {
    Write-Host "Configurando remote 'origin'..." -ForegroundColor Yellow
    git remote add origin $remoteUrl
    Write-Host "Remote configurado: $remoteUrl" -ForegroundColor Green
} else {
    Write-Host "Remote já configurado: $currentRemote" -ForegroundColor Green
}

# Criar branch develop se não existir
$developExists = git branch --list develop

if (-not $developExists) {
    Write-Host "Criando branch 'develop'..." -ForegroundColor Yellow
    git checkout -b develop
    Write-Host "Branch 'develop' criada" -ForegroundColor Green
} else {
    Write-Host "Branch 'develop' já existe" -ForegroundColor Green
    git checkout develop 2>$null
}

# Configurar branch padrão
git config init.defaultBranch main

Write-Host "`nGit Flow configurado com sucesso!" -ForegroundColor Green
Write-Host "`nEstrutura de branches:" -ForegroundColor Cyan
Write-Host "  - main (produção)" -ForegroundColor White
Write-Host "  - develop (desenvolvimento)" -ForegroundColor White
Write-Host "`nPróximos passos:" -ForegroundColor Cyan
Write-Host "  1. Adicione seus arquivos: git add ." -ForegroundColor White
Write-Host "  2. Faça commit inicial: git commit -m 'feat: commit inicial do projeto'" -ForegroundColor White
Write-Host "  3. Push para main: git checkout main && git push -u origin main" -ForegroundColor White
Write-Host "  4. Push develop: git checkout develop && git push -u origin develop" -ForegroundColor White

