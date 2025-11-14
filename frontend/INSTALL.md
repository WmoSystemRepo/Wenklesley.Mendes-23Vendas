# Instalação do Frontend Angular

## Pré-requisitos

- Node.js 18+ instalado
- npm ou yarn

## Instalação

```powershell
cd frontend
npm install
```

## Executar em Desenvolvimento

```powershell
npm start
```

Acesse: http://localhost:4200

## Build para Produção

```powershell
npm run build:prod
```

Os arquivos serão gerados em `dist/123vendas-dashboard/`

## Com Docker

Não é necessário instalar dependências localmente. O Docker fará isso automaticamente:

```powershell
# Na raiz do projeto
docker-compose up frontend
```

## Troubleshooting

### Erro: Cannot find module

```powershell
# Limpar e reinstalar
rm -rf node_modules package-lock.json
npm install
```

### Erro: Porta 4200 em uso

Altere a porta no `angular.json` ou use:

```powershell
ng serve --port 4201
```

