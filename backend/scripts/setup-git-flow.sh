#!/bin/bash
# Script para configurar Git Flow no repositório
# Execute este script após inicializar o repositório Git

echo "Configurando Git Flow..."

# Verificar se Git está instalado
if ! command -v git &> /dev/null; then
    echo "ERRO: Git não está instalado"
    exit 1
fi

# Verificar se estamos em um repositório Git
if [ ! -d .git ]; then
    echo "Inicializando repositório Git..."
    git init
    git branch -M main
fi

# Configurar template de commit
if [ -f .gitmessage ]; then
    git config commit.template .gitmessage
    echo "Template de commit configurado"
fi

# Configurar remote (se ainda não estiver configurado)
REMOTE_URL="https://github.com/WmoSystemRepo/Teste_Admiss-o_Backend.git"
CURRENT_REMOTE=$(git remote get-url origin 2>/dev/null)

if [ $? -ne 0 ]; then
    echo "Configurando remote 'origin'..."
    git remote add origin "$REMOTE_URL"
    echo "Remote configurado: $REMOTE_URL"
else
    echo "Remote já configurado: $CURRENT_REMOTE"
fi

# Criar branch develop se não existir
if ! git branch --list develop | grep -q develop; then
    echo "Criando branch 'develop'..."
    git checkout -b develop
    echo "Branch 'develop' criada"
else
    echo "Branch 'develop' já existe"
    git checkout develop 2>/dev/null
fi

# Configurar branch padrão
git config init.defaultBranch main

echo ""
echo "Git Flow configurado com sucesso!"
echo ""
echo "Estrutura de branches:"
echo "  - main (produção)"
echo "  - develop (desenvolvimento)"
echo ""
echo "Próximos passos:"
echo "  1. Adicione seus arquivos: git add ."
echo "  2. Faça commit inicial: git commit -m 'feat: commit inicial do projeto'"
echo "  3. Push para main: git checkout main && git push -u origin main"
echo "  4. Push develop: git checkout develop && git push -u origin develop"

