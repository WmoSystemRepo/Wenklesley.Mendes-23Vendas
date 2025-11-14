# Sistema de Vendas - 123Vendas

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=c-sharp)
![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?logo=microsoft-sql-server)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)
![Tests](https://img.shields.io/badge/Tests-38%20Passing-28A745)
![Coverage](https://img.shields.io/badge/Coverage-100%25%20Domain-28A745)

Sistema de gerenciamento de vendas desenvolvido em **.NET 8** seguindo **Clean Architecture**, **Domain-Driven Design (DDD)**, com testes unitários completos e boas práticas de engenharia de software.

## 📋 Índice

- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Como Executar](#como-executar)
- [Como Testar](#como-testar)
- [Docker](#docker)
- [Migrations](#migrations)
- [Endpoints da API](#endpoints-da-api)
- [Regras de Negócio](#regras-de-negócio)
- [Eventos de Domínio](#eventos-de-domínio)
- [Observabilidade](#observabilidade)
- [Versionamento](#versionamento)

## 🛠 Tecnologias

- **.NET 8**
- **Entity Framework Core 8.0**
- **SQL Server**
- **MediatR** (CQRS)
- **FluentValidation**
- **Serilog** (Logging em JSON)
- **Swagger/OpenAPI**
- **XUnit** + **Shouldly** + **Bogus** + **NSubstitute** (Testes)
- **Docker** + **Docker Compose**

## 🏗 Arquitetura

O projeto segue os princípios da **Clean Architecture** e **DDD**, com separação clara de responsabilidades:

```
┌─────────────────────────────────────────────────────────┐
│                        API Layer                         │
│  (Controllers, Middlewares, DI, Swagger)                 │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│                    Application Layer                     │
│  (DTOs, Commands/Queries, Handlers, Validators)         │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│                      Domain Layer                        │
│  (Entities, Aggregates, Value Objects, Domain Events)   │
└──────────────────────┬──────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────┐
│                  Infrastructure Layer                     │
│  (EF Core, Repositories, Migrations, Serilog)            │
└──────────────────────────────────────────────────────────┘
```

### Camadas

- **Domain**: Entidades, agregados, value objects, eventos de domínio, regras de negócio
- **Application**: DTOs, interfaces, handlers (CQRS), validações
- **Infra**: EF Core, repositórios, migrations, Serilog
- **Infra.IoC**: Configuração de Dependency Injection
- **Api**: Controllers, middlewares, Program.cs, Swagger
- **Tests**: Testes unitários com 100% de cobertura do domínio

## 📁 Estrutura do Projeto

```
123Vendas/
├── backend/
│   ├── src/
│   │   ├── Api/                    # Controllers, Middlewares, DI, Swagger
│   │   ├── Application/            # DTOs, Interfaces, Handlers, Validações
│   │   ├── Domain/                 # Entidades, Agregados, VOs, Regras, Eventos
│   │   ├── Infra/                  # EF Core, Repositórios, Migrations, Serilog
│   │   └── Infra.IoC/              # Dependency Injection
│   ├── tests/
│   └── UnitTests/              # XUnit + Shouldly + Bogus + NSubstitute
├── Dockerfile
├── docker-compose.yml
└── README.md
```

## 🚀 Como Executar

### Pré-requisitos

- .NET 8 SDK
- SQL Server (ou usar Docker)
- Visual Studio 2022 / VS Code / Rider

### Passo a Passo

1. **Clone o repositório**
   ```bash
   git clone <repository-url>
   cd 123Vendas
   ```

2. **Restore das dependências**
   ```bash
   dotnet restore
   ```

3. **Configure a connection string** no arquivo `backend/src/Api/appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=localhost,1433;Database=123Vendas;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
     }
   }
   ```

4. **Execute as migrations**
   ```bash
   cd backend/src/Infra
   dotnet ef migrations add InitialCreate --startup-project ../Api
   dotnet ef database update --startup-project ../Api
   ```

5. **Execute a API**
   ```bash
   cd backend/src/Api
   dotnet run
   ```

6. **Acesse o Swagger**
   - URL: `https://localhost:5001/swagger` (ou porta configurada)

## 🧪 Como Testar

### Executar todos os testes

```bash
dotnet test
```

### Executar testes com cobertura

O projeto está configurado para gerar relatórios de cobertura automaticamente. Você pode executar de duas formas:

#### Opção 1: Usando script (recomendado)

```bash
# Windows (PowerShell)
.\scripts\test-with-coverage.ps1

# Linux/Mac (Bash)
chmod +x scripts/test-with-coverage.sh
./scripts/test-with-coverage.sh
```

#### Opção 2: Comando direto

```bash
cd tests/UnitTests
dotnet test --collect:"XPlat Code Coverage"
```

Os relatórios serão gerados em `tests/UnitTests/TestResults/` nos formatos:
- **OpenCover** (`coverage.opencover.xml`) - Para integração com ferramentas como SonarQube
- **Cobertura** (`coverage.cobertura.xml`) - Para integração com Azure DevOps
- **JSON** (`coverage.json`) - Para análise programática

### Visualizar relatórios HTML

Para gerar relatórios HTML visualizáveis:

```bash
# Instalar ReportGenerator (uma vez)
dotnet tool install -g dotnet-reportgenerator-globaltool

# Gerar relatório HTML
reportgenerator \
  -reports:tests/UnitTests/TestResults/coverage.opencover.xml \
  -targetdir:tests/UnitTests/TestResults/html \
  -reporttypes:Html
```

Depois, abra `tests/UnitTests/TestResults/html/index.html` no navegador.

### Metas de Cobertura

- **Domain Layer**: 100% (obrigatório)
- **Application Layer**: > 80% (recomendado)
- **Infrastructure Layer**: > 70% (recomendado)

### Testes por camada

- **Domain Tests**: Testes de entidades, value objects, serviços de domínio
- **Application Tests**: Testes de handlers, validadores

## 🧪 Testes

### Executar Testes no Docker (Recomendado)

Para executar todos os testes usando Docker:

```powershell
# Executar todos os testes
.\scripts\run-tests-docker.ps1

# Executar apenas testes unitários
.\scripts\run-tests-docker.ps1 -TestType unit

# Executar apenas testes de integração
.\scripts\run-tests-docker.ps1 -TestType integration

# Executar apenas testes BDD
.\scripts\run-tests-docker.ps1 -TestType bdd
```

**Documentação completa**: Veja [TESTES_DOCKER.md](./TESTES_DOCKER.md) para instruções detalhadas.

### Executar Testes Localmente

```bash
# Testes unitários
dotnet test tests/UnitTests/UnitTests.csproj

# Testes de integração (requer SQL Server na porta 1434)
dotnet test tests/IntegrationTests/IntegrationTests.csproj

# Testes BDD (requer SQL Server na porta 1434)
dotnet test tests/BddTests/BddTests.csproj
```

### Tipos de Testes

- **Testes Unitários**: 38 testes - Testam lógica de negócio isolada
- **Testes de Integração**: 57 testes - Testam integração entre camadas
- **Testes BDD**: 23 testes - Testes baseados em comportamento (SpecFlow)

## 🐳 Docker

### Executar Ambiente Completo (API + Frontend)

1. **Execute o docker-compose**
   ```bash
   docker-compose up --build
   ```

2. **Aguarde os serviços iniciarem** (SQL Server precisa estar healthy)

3. **Acesse as aplicações**
   - **Frontend Angular**: http://localhost:4200
   - **API .NET**: http://localhost:5000
   - **Swagger**: http://localhost:5000/swagger

### Executar Apenas API

```bash
docker-compose up api sqlserver
```

### Executar Apenas Frontend

```bash
docker-compose up frontend
```

### Parar os serviços

```bash
docker-compose down
```

### Remover volumes (limpar banco de dados)

```bash
docker-compose down -v
```

**📚 Documentação completa**: Veja [DOCKER_SETUP.md](./DOCKER_SETUP.md) para instruções detalhadas.

## 📊 Migrations

### Criar nova migration

```bash
cd backend/src/Infra
dotnet ef migrations add NomeDaMigration --startup-project ../Api
```

### Aplicar migrations

```bash
dotnet ef database update --startup-project ../Api
```

### Reverter migration

```bash
dotnet ef database update NomeDaMigrationAnterior --startup-project ../Api
```

## 🔌 Endpoints da API

### Base URL
```
http://localhost:5000/api/venda
```

### 📝 Padrão de Resposta

Todos os endpoints retornam um padrão consistente:

**Sucesso:**
```json
{
  "success": true,
  "data": { ... },
  "message": "Operação realizada com sucesso",
  "timestamp": "2024-01-15T10:30:00Z",
  "correlationId": "550e8400-e29b-41d4-a716-446655440000"
}
```

**Erro:**
```json
{
  "success": false,
  "message": "Mensagem de erro",
  "errors": ["Lista de erros de validação"],
  "timestamp": "2024-01-15T10:30:00Z",
  "correlationId": "550e8400-e29b-41d4-a716-446655440000"
}
```

### GET /api/venda
Lista todas as vendas.

**Resposta:**
```json
{
  "success": true,
  "data": [
    {
      "id": "guid",
      "numeroVenda": "V001",
      "data": "2024-01-15T10:30:00Z",
      "clienteId": "guid",
      "clienteNome": "Cliente Teste",
      "filialId": "guid",
      "filialNome": "Filial Teste",
      "status": "NaoCancelado",
      "valorTotal": 450.00,
      "itens": [
        {
          "id": "guid",
          "produtoId": "guid",
          "produtoNome": "Produto Teste",
          "quantidade": 5,
          "valorUnitario": 100.00,
          "desconto": 50.00,
          "valorTotalItem": 450.00
        }
      ]
    }
  ],
  "timestamp": "2024-01-15T10:30:00Z",
  "correlationId": "550e8400-e29b-41d4-a716-446655440000"
}
```

### GET /api/venda/{id}
Obtém uma venda por ID.

**Resposta:** (mesmo formato do GET /api/venda, mas objeto único no campo `data`)

### POST /api/venda
Cria uma nova venda.

**Request:**
```json
{
  "numeroVenda": "V001",
  "clienteId": "guid",
  "clienteNome": "Cliente Teste",
  "filialId": "guid",
  "filialNome": "Filial Teste",
  "itens": [
    {
      "produtoId": "guid",
      "produtoNome": "Produto Teste",
      "quantidade": 5,
      "valorUnitario": 100.00
    }
  ]
}
```

**Resposta:** `201 Created` com o ID da venda criada

### PUT /api/venda/{id}
Atualiza uma venda existente.

**Request:**
```json
{
  "itensParaAdicionar": [
    {
      "produtoId": "guid",
      "produtoNome": "Novo Produto",
      "quantidade": 3,
      "valorUnitario": 50.00
    }
  ],
  "itensParaRemover": ["guid-item-id"],
  "itensParaAtualizar": [
    {
      "itemId": "guid",
      "quantidade": 10,
      "valorUnitario": 100.00
    }
  ]
}
```

**Resposta:** `204 No Content`

### DELETE /api/venda/{id}
Cancela uma venda (soft delete).

**Resposta:** `204 No Content`

## 📏 Regras de Negócio

### Descontos por Quantidade (aplicados por item)

As regras de desconto são aplicadas **por item individual** da venda, baseadas na quantidade de cada produto:

- **Quantidade > 20**: ❌ **Proibido** (lança exceção `InvalidOperationException`)
- **Quantidade entre 10 e 20 (inclusive)**: ✅ **20% de desconto automático**
- **Quantidade acima de 4 (> 4) e até 9**: ✅ **10% de desconto automático**
- **Quantidade até 4 (<= 4)**: ✅ **Sem desconto** (não é possível aplicar desconto)

#### Interpretação da Regra "Acima de 4 itens"

O requisito especifica: *"Compras acima de 4 itens iguais tem 10% de desconto automaticamente"*.

**Interpretação implementada**: "Acima de 4" foi interpretado como **"maior que 4"** (`> 4`), ou seja:
- ✅ **5 itens ou mais** (até 9): Aplica 10% de desconto
- ❌ **4 itens ou menos**: Não aplica desconto (conforme regra: "Compras abaixo de 4 itens não podem ter desconto")

Esta interpretação garante que:
1. A regra "Compras abaixo de 4 itens não podem ter desconto" é respeitada
2. O desconto é aplicado apenas quando a quantidade é estritamente maior que 4
3. Há uma distinção clara entre 4 itens (sem desconto) e 5 itens (com desconto)

### Exemplos de Cálculo

| Quantidade | Valor Unitário | Valor Bruto | Desconto | Valor Total |
|------------|----------------|-------------|----------|-------------|
| 3 | R$ 100,00 | R$ 300,00 | R$ 0,00 | R$ 300,00 |
| 4 | R$ 100,00 | R$ 400,00 | R$ 0,00 | R$ 400,00 |
| 5 | R$ 100,00 | R$ 500,00 | R$ 50,00 (10%) | R$ 450,00 |
| 15 | R$ 100,00 | R$ 1.500,00 | R$ 300,00 (20%) | R$ 1.200,00 |
| 21 | R$ 100,00 | - | ❌ Proibido | - |

## 🔔 Eventos de Domínio

O sistema emite os seguintes eventos de domínio (logados via Serilog em JSON):

1. **CompraEfetuada**: Quando o primeiro item é adicionado à venda
2. **CompraAlterada**: Quando itens são atualizados
3. **CompraCancelada**: Quando a venda é cancelada
4. **ItemCancelado**: Quando um item é removido

### Exemplo de Log (JSON)

```json
{
  "Timestamp": "2024-01-15T10:30:00.123Z",
  "Level": "Information",
  "MessageTemplate": "Evento de Domínio: CompraEfetuada - VendaId: {VendaId}, NumeroVenda: {NumeroVenda}, ValorTotal: {ValorTotal}",
  "Properties": {
    "VendaId": "guid",
    "NumeroVenda": "V001",
    "ValorTotal": 450.00
  }
}
```

## 🔍 Observabilidade

O sistema implementa observabilidade para identificar gargalos através de:

- **Logs estruturados em JSON**: Todos os logs via Serilog em formato JSON
- **Middleware de Performance**: Mede tempo de resposta de cada requisição e gera Correlation IDs
- **Health Checks**: Endpoint `/health` com métricas de saúde da aplicação
- **Eventos de Domínio**: Logados automaticamente para rastreamento do fluxo de negócio

**Como identificar gargalos:**
- Buscar logs com `ElapsedMs > 1000` para encontrar requisições lentas
- Usar Correlation IDs (header `X-Correlation-Id`) para rastrear requisições específicas
- Monitorar health checks para detectar problemas de infraestrutura
- Analisar eventos de domínio para entender o fluxo de negócio

---

## 📝 Versionamento

Este projeto segue:

- **Git Flow Workflow**: Estrutura de branches documentada em [CONTRIBUTING.md](CONTRIBUTING.md)
- **Commit Semântico**: Padrão Conventional Commits documentado em [CONTRIBUTING.md](CONTRIBUTING.md)

