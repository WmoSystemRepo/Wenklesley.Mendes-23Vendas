# Docker Setup - 123Vendas

Documentação completa para executar o ambiente Docker com API .NET e Frontend Angular.

## 📋 Pré-requisitos

- Docker Desktop instalado e rodando
- Docker Compose instalado
- PowerShell (Windows) ou Bash (Linux/Mac)

## 🚀 Executar Ambiente Completo

### Opção 1: Todos os Serviços (Recomendado)

Execute todos os serviços (SQL Server, API e Frontend) de uma vez:

```powershell
docker-compose up --build
```

Este comando irá:
1. ✅ Subir SQL Server na porta 1433
2. ✅ Subir API .NET na porta 5000
3. ✅ Subir Frontend Angular na porta 4200
4. ✅ Configurar rede interna entre serviços

### Opção 2: Serviços Individuais

```powershell
# Apenas SQL Server
docker-compose up sqlserver

# Apenas API
docker-compose up api

# Apenas Frontend
docker-compose up frontend
```

## 🌐 Acessar Aplicação

Após subir os serviços:

- **Frontend Angular**: http://localhost:4200
- **API .NET**: http://localhost:5000
- **Swagger**: http://localhost:5000/swagger
- **Health Check**: http://localhost:5000/health
- **SQL Server**: localhost:1433

## 📁 Estrutura de Serviços

### SQL Server
- **Container**: `123vendas-sqlserver`
- **Porta**: 1433
- **Credenciais**:
  - Usuário: `sa`
  - Senha: `YourStrong@Passw0rd`
  - Database: `123Vendas`

### API .NET
- **Container**: `123vendas-api`
- **Porta**: 5000
- **Ambiente**: Development
- **Dependências**: SQL Server

### Frontend Angular
- **Container**: `123vendas-frontend`
- **Porta**: 4200
- **Servidor**: Nginx
- **Dependências**: API

## 🔧 Comandos Úteis

### Ver logs dos serviços

```powershell
# Todos os serviços
docker-compose logs -f

# Serviço específico
docker-compose logs -f api
docker-compose logs -f frontend
docker-compose logs -f sqlserver
```

### Parar serviços

```powershell
# Parar todos
docker-compose down

# Parar e remover volumes (limpar banco)
docker-compose down -v
```

### Rebuild de um serviço

```powershell
# Rebuild e restart
docker-compose up --build --force-recreate frontend
```

### Executar comandos dentro do container

```powershell
# Executar migrations na API
docker-compose exec api dotnet ef database update --project /src/Infra --startup-project /src/Api

# Acessar shell do frontend
docker-compose exec frontend sh

# Acessar SQL Server
docker-compose exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd
```

## 🛠️ Desenvolvimento

### Desenvolvimento Local (sem Docker)

Se preferir desenvolver localmente:

#### Frontend
```powershell
cd frontend
npm install
npm start
# Acesse http://localhost:4200
```

#### API
```powershell
cd backend/src/Api
dotnet run
# Acesse http://localhost:5000
```

### Hot Reload

Para desenvolvimento com hot reload:

1. Execute apenas SQL Server no Docker:
   ```powershell
   docker-compose up sqlserver
   ```

2. Execute API e Frontend localmente:
   ```powershell
   # Terminal 1 - API
   cd backend/src/Api
   dotnet watch run

   # Terminal 2 - Frontend
   cd frontend
   npm start
   ```

## 🔍 Troubleshooting

### Porta já em uso

Se alguma porta estiver em uso, altere no `docker-compose.yml`:

```yaml
ports:
  - "5001:80"  # Altere 5000 para 5001
```

### Frontend não conecta na API

Verifique:
1. API está rodando: `docker-compose ps`
2. CORS está configurado em `appsettings.json`
3. URL da API no frontend está correta (`frontend/app/services/api.service.ts`)

### SQL Server não inicia

```powershell
# Ver logs
docker-compose logs sqlserver

# Verificar se está saudável
docker ps --filter "name=123vendas-sqlserver"

# Reiniciar
docker-compose restart sqlserver
```

### Frontend não compila

```powershell
# Limpar cache e rebuild
docker-compose build --no-cache frontend
docker-compose up frontend
```

## 📊 Health Checks

Todos os serviços têm health checks configurados:

```powershell
# Verificar status
docker-compose ps

# Verificar health
docker inspect --format='{{.State.Health.Status}}' 123vendas-api
docker inspect --format='{{.State.Health.Status}}' 123vendas-frontend
docker inspect --format='{{.State.Health.Status}}' 123vendas-sqlserver
```

## 🔐 Variáveis de Ambiente

### API

Variáveis configuradas no `docker-compose.yml`:
- `ASPNETCORE_ENVIRONMENT=Development`
- `ConnectionStrings__DefaultConnection` (configurado automaticamente)

### Frontend

O frontend usa a URL da API configurada em:
- `frontend/app/services/api.service.ts` (baseUrl)

Para produção, configure via variáveis de ambiente no Docker.

## 📝 Notas Importantes

1. **Primeira Execução**: O build pode levar alguns minutos
2. **Migrations**: Executadas automaticamente na inicialização da API
3. **Volumes**: Dados do SQL Server são persistidos em volume Docker
4. **Rede**: Todos os serviços estão na mesma rede Docker (`vendas-network`)

## 🔗 Documentação Relacionada

- [README.md](./README.md) - Documentação geral
- [TESTES_DOCKER.md](./TESTES_DOCKER.md) - Executar testes no Docker
- [DOCUMENTACAO_TESTES.md](./DOCUMENTACAO_TESTES.md) - Documentação de testes

## 🎯 Próximos Passos

Após subir o ambiente:

1. Acesse http://localhost:4200 para ver o dashboard
2. Explore as abas: Testes, Logs, Git Flow, API Simulator, Validação
3. Teste os endpoints da API através do simulador
4. Execute os testes através do dashboard

---

**Última atualização**: 2024-01-15

