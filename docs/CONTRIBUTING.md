# Guia de Contribuição - 123Vendas

Este documento descreve os padrões e processos para contribuir com o projeto.

## 📋 Índice

- [Git Flow Workflow](#git-flow-workflow)
- [Commit Semântico](#commit-semântico)
- [Padrões de Código](#padrões-de-código)
- [Testes](#testes)

---

## 🔀 Git Flow Workflow

Este projeto utiliza o **Git Flow** para gerenciamento de branches e releases.

### Estrutura de Branches

```
main (produção)
  └── develop (desenvolvimento)
       ├── feature/nome-da-funcionalidade
       ├── bugfix/nome-do-bug
       └── hotfix/nome-da-correcao-urgente
```

### Tipos de Branches

#### `main`
- Branch principal de produção
- Contém apenas código estável e testado
- Protegida (não permite push direto)
- Releases são feitas a partir desta branch

#### `develop`
- Branch de desenvolvimento
- Contém código em desenvolvimento
- Integração de features antes de ir para produção

#### `feature/*`
- Branches para novas funcionalidades
- Criadas a partir de `develop`
- Nomenclatura: `feature/nome-descritivo`
- Exemplos:
  - `feature/adicionar-filtro-vendas`
  - `feature/implementar-paginacao`
  - `feature/adicionar-autenticacao`

#### `bugfix/*`
- Branches para correção de bugs
- Criadas a partir de `develop`
- Nomenclatura: `bugfix/nome-descritivo`
- Exemplos:
  - `bugfix/corrigir-calculo-desconto`
  - `bugfix/corrigir-validacao-quantidade`

#### `hotfix/*`
- Branches para correções urgentes em produção
- Criadas a partir de `main`
- Merge direto em `main` e `develop`
- Nomenclatura: `hotfix/nome-descritivo`
- Exemplos:
  - `hotfix/corrigir-vulnerabilidade-seguranca`
  - `hotfix/corrigir-erro-critico`

### Configuração Inicial do Git Flow

**Primeira vez configurando o projeto:**

1. **Inicializar repositório Git** (se ainda não estiver inicializado):
   ```bash
   git init
   git branch -M main
   ```

2. **Executar script de setup** (configura Git Flow automaticamente):
   ```bash
   # Windows (PowerShell)
   .\scripts\setup-git-flow.ps1
   
   # Linux/Mac (Bash)
   chmod +x scripts/setup-git-flow.sh
   ./scripts/setup-git-flow.sh
   ```

3. **Fazer commit inicial e push:**
   ```bash
   git add .
   git commit -m "feat: commit inicial do projeto 123Vendas"
   git remote add origin https://github.com/WmoSystemRepo/Teste_Admiss-o_Backend.git
   git push -u origin main
   git checkout develop
   git push -u origin develop
   ```

### Workflow de Desenvolvimento

#### Criar uma Feature

```bash
# 1. Atualizar develop
git checkout develop
git pull origin develop

# 2. Criar branch da feature
git checkout -b feature/minha-feature

# 3. Desenvolver e commitar (usando template)
git add .
git commit  # Abre editor com template preenchido
# Ou usar flag -m: git commit -m "feat(venda): adiciona filtro por data"

# 4. Push da feature
git push origin feature/minha-feature

# 5. Criar Pull Request para develop
# (via GitHub/GitLab interface)

# 6. Após merge, deletar branch local
git checkout develop
git pull origin develop
git branch -d feature/minha-feature
```

#### Criar um Hotfix

```bash
# 1. Criar branch do hotfix a partir de main
git checkout main
git pull origin main
git checkout -b hotfix/corrigir-bug-critico

# 2. Corrigir e commitar
git add .
git commit -m "fix(venda): corrige erro crítico no cálculo"

# 3. Merge em main e develop
git checkout main
git merge hotfix/corrigir-bug-critico
git push origin main

git checkout develop
git merge hotfix/corrigir-bug-critico
git push origin develop

# 4. Deletar branch
git branch -d hotfix/corrigir-bug-critico
```

---

## 📝 Commit Semântico

Utilizamos **Conventional Commits** para padronizar mensagens de commit.

### Formato

```
<tipo>(<escopo>): <descrição>

[corpo opcional]

[rodapé opcional]
```

### Tipos de Commit

| Tipo | Descrição | Exemplo |
|------|-----------|---------|
| `feat` | Nova funcionalidade | `feat(venda): adiciona endpoint de listagem` |
| `fix` | Correção de bug | `fix(desconto): corrige cálculo para 4 itens` |
| `docs` | Documentação | `docs(readme): atualiza instruções de execução` |
| `style` | Formatação (não afeta código) | `style(api): formata código` |
| `refactor` | Refatoração | `refactor(venda): extrai lógica de cálculo` |
| `test` | Testes | `test(venda): adiciona testes de validação` |
| `chore` | Tarefas de manutenção | `chore(deps): atualiza dependências` |
| `perf` | Melhoria de performance | `perf(api): otimiza consulta de vendas` |
| `ci` | CI/CD | `ci(github): adiciona workflow de testes` |

### Escopo

O escopo é opcional e indica a área afetada:

- `venda` - Funcionalidades de venda
- `desconto` - Regras de desconto
- `api` - Camada de API
- `domain` - Camada de domínio
- `infra` - Infraestrutura
- `test` - Testes

### Exemplos

#### Commit Simples

```bash
feat(venda): adiciona endpoint de criação de venda
```

#### Commit com Corpo

```bash
fix(desconto): corrige cálculo de desconto para 4 itens

O requisito especifica "acima de 4 itens", que significa
maior que 4 (> 4), não maior ou igual (>= 4).

Ajusta a validação de Quantidade > 4 para aplicar desconto.
```

#### Commit com Breaking Change

```bash
feat(api): altera formato de resposta do endpoint de vendas

BREAKING CHANGE: O campo 'valorTotal' agora retorna como 'valorTotalVenda'
```

#### Commit com Múltiplos Tipos

```bash
feat(venda): adiciona filtro por data
test(venda): adiciona testes do filtro
docs(readme): documenta novo endpoint
```

### Configuração do Template de Commit

O projeto inclui um template de commit (`.gitmessage`) para facilitar o uso de commits semânticos.

**Configurar o template:**
```bash
git config commit.template .gitmessage
```

Após configurar, ao executar `git commit` sem a flag `-m`, o editor abrirá com o template preenchido.

**Configuração Automática:**
Execute o script de setup do Git Flow que já configura o template automaticamente:
```bash
# Windows (PowerShell)
.\scripts\setup-git-flow.ps1

# Linux/Mac (Bash)
chmod +x scripts/setup-git-flow.sh
./scripts/setup-git-flow.sh
```

### Boas Práticas

1. ✅ Use o presente do indicativo: "adiciona" não "adicionou"
2. ✅ Seja específico na descrição
3. ✅ Limite a primeira linha a 72 caracteres
4. ✅ Use o corpo para explicar o "porquê", não o "o quê"
5. ✅ Referencie issues/PRs quando aplicável: `Closes #123`
6. ✅ Use o template de commit para manter consistência

### Exemplos de Commits do Projeto

```bash
# Feature
feat(venda): implementa CRUD completo de vendas
feat(domain): adiciona eventos de domínio (CompraEfetuada, CompraAlterada)

# Fix
fix(desconto): corrige regra de desconto para quantidade > 4
fix(api): corrige erro 404 na rota raiz

# Test
test(venda): adiciona testes de regras de desconto
test(domain): adiciona testes de eventos de domínio

# Docs
docs(readme): adiciona instruções de execução com Docker
docs(contributing): documenta Git Flow e commits semânticos

# Refactor
refactor(venda): extrai lógica de cálculo para serviço de domínio
refactor(api): reorganiza estrutura de middlewares

# Chore
chore(deps): atualiza Serilog para versão 3.1.1
chore(ci): configura GitHub Actions para testes
```

---

## 💻 Padrões de Código

### Clean Code

- Nomes descritivos e significativos
- Funções pequenas e com responsabilidade única
- Evitar comentários desnecessários (código auto-explicativo)
- DRY (Don't Repeat Yourself)

### SOLID

- **S**ingle Responsibility Principle
- **O**pen/Closed Principle
- **L**iskov Substitution Principle
- **I**nterface Segregation Principle
- **D**ependency Inversion Principle

### Object Calisthenics

- Um nível de indentação por método
- Não use ELSE
- Encapsule primitivos e strings
- Coleções de primeira classe
- Um ponto por linha
- Não abrevie
- Mantenha entidades pequenas
- Sem classes com mais de 2 variáveis de instância
- Sem getters/setters/propriedades

---

## 🧪 Testes

### Cobertura

- **100% de cobertura** da lógica de domínio é obrigatório
- Testes devem seguir o padrão AAA (Arrange, Act, Assert)

### Estrutura

```csharp
[Fact]
public void NomeDoTeste_Contexto_ResultadoEsperado()
{
    // Arrange
    var item = new VendaItem(...);

    // Act
    item.AtualizarQuantidade(10);

    // Assert
    item.Quantidade.ShouldBe(10);
}
```

### Ferramentas

- **XUnit** - Framework de testes
- **Shouldly** - Assertions fluentes
- **Bogus** - Geração de dados fake
- **NSubstitute** - Mocking

---

## 📚 Recursos

- [Conventional Commits](https://www.conventionalcommits.org/)
- [Git Flow](https://nvie.com/posts/a-successful-git-branching-model/)
- [Clean Code](https://www.amazon.com/Clean-Code-Handbook-Software-Craftsmanship/dp/0132350882)

---

## ❓ Dúvidas?

Se tiver dúvidas sobre o processo de contribuição, abra uma issue ou entre em contato com a equipe.

