# 123Vendas Dashboard - Frontend Angular

Dashboard Angular profissional para visualização de testes, logs, Git Flow e simulação de APIs.

## 🚀 Início Rápido

### Com Docker (Recomendado)

```powershell
# Na raiz do projeto
docker-compose up frontend
```

Acesse: http://localhost:4200

### Desenvolvimento Local

```powershell
# Instalar dependências
npm install

# Executar em desenvolvimento
npm start

# Build para produção
npm run build:prod
```

## 📁 Estrutura do Projeto

```
frontend/
├── src/
│   ├── app/
│   │   ├── components/
│   │   │   ├── dashboard/          # Componente principal
│   │   │   │   ├── dashboard.component.ts
│   │   │   │   └── tabs/            # Abas do dashboard
│   │   │   │       ├── tests-tab/      # Aba de Testes
│   │   │   │       ├── logs-tab/       # Aba de Logs
│   │   │   │       ├── git-flow-tab/   # Aba Git Flow
│   │   │   │       ├── api-simulator-tab/ # Aba API Simulator
│   │   │   │       └── validation-tab/   # Aba Validação
│   │   │   └── layout/              # Componentes de layout
│   │   ├── services/                 # Serviços Angular
│   │   │   ├── api.service.ts
│   │   │   ├── test.service.ts
│   │   │   ├── logs.service.ts
│   │   │   └── git.service.ts
│   │   └── models/                  # Interfaces TypeScript
│   └── styles/                      # Estilos globais
└── Dockerfile                       # Docker multi-stage
```

## 🎨 Funcionalidades

### Aba Testes
- Visualização de testes em tempo real
- Filtros por tipo (unit, integration, BDD)
- Estatísticas e gráficos
- Executar testes

### Aba Logs
- Stream de logs JSON em tempo real
- Filtros por nível (Info, Warning, Error)
- Busca em logs
- Auto-scroll
- Syntax highlighting

### Aba Git Flow
- Diagrama visual do Git Flow
- Tipos de branches explicados
- Validador de commits semânticos
- Exemplos práticos

### Aba API Simulator
- Interface tipo Postman
- Testar todos os endpoints REST
- Editor JSON
- Histórico de requisições
- Templates pré-configurados

### Aba Validação
- Checklist de requisitos
- Score de conformidade
- Links para documentação

## 🛠️ Tecnologias

- **Angular 17**: Framework principal
- **Angular Material**: Componentes UI
- **RxJS**: Programação reativa
- **TypeScript**: Tipagem estática
- **SCSS**: Estilos com variáveis e mixins
- **Nginx**: Servidor web (produção)

## 📦 Dependências Principais

- `@angular/material`: Componentes Material Design
- `@angular/cdk`: Componentes base
- `rxjs`: Programação reativa
- `chart.js` / `ng2-charts`: Gráficos (opcional)

## 🔧 Configuração

### URL da API

Configure a URL da API em `src/app/services/api.service.ts`:

```typescript
private readonly baseUrl = 'http://localhost:5000';
```

Para Docker, use: `http://api:80` (dentro da rede Docker)

### CORS

A API deve ter CORS configurado para permitir requisições do frontend.

## 🐳 Docker

### Build

```powershell
docker build -t 123vendas-frontend ./frontend
```

### Run

```powershell
docker run -p 4200:80 123vendas-frontend
```

## 📝 Boas Práticas Implementadas

1. **Arquitetura**
   - Componentes reutilizáveis
   - Services para lógica de negócio
   - Models para tipagem
   - Separação de responsabilidades

2. **Performance**
   - OnPush change detection
   - TrackBy functions
   - Lazy loading (quando aplicável)
   - Virtual scrolling (quando necessário)

3. **UX/UI**
   - Material Design
   - Responsive design
   - Loading states
   - Error handling
   - Feedback visual

4. **Código**
   - TypeScript strict mode
   - Interfaces e tipos
   - Clean code
   - Comentários quando necessário

## 🧪 Testes

```powershell
# Unit tests
npm test

# E2E tests (se configurado)
npm run e2e
```

## 📚 Documentação

- [Angular Docs](https://angular.io/docs)
- [Angular Material](https://material.angular.io/)
- [RxJS](https://rxjs.dev/)

---

**Desenvolvido com Angular 17 e Material Design**

