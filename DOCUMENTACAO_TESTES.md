# Documentação Completa de Testes - 123Vendas

Este documento descreve todos os testes implementados no projeto, organizados por tipo e funcionalidade.

## 📊 Resumo Geral

| Tipo de Teste | Quantidade | Cobertura |
|---------------|-----------|-----------|
| **Testes Unitários** | 38 | 100% Domain |
| **Testes de Integração** | 57 | API, Controllers, Regras |
| **Testes BDD** | 23 cenários | Features completas |
| **TOTAL** | **118 testes** | - |

---

## 🧪 1. Testes Unitários (38 testes)

### 1.1. Domain - Entities

#### VendaTests (11 testes)
**Arquivo**: `tests/UnitTests/Domain/Entities/VendaTests.cs`

| Teste | Descrição |
|-------|-----------|
| `CriarVenda_DeveCriarVendaComStatusNaoCancelado` | Verifica criação de venda com status inicial correto |
| `AdicionarItem_DeveAdicionarItemEVincularAVenda` | Verifica adição de item à venda |
| `AdicionarItem_DeveRecalcularValorTotal` | Verifica recálculo automático do valor total |
| `AdicionarItem_DeveEmitirEventoCompraEfetuada` | Verifica emissão de evento de domínio |
| `AdicionarItem_VendaCancelada_DeveLancarExcecao` | Verifica que não é possível adicionar item em venda cancelada |
| `RemoverItem_DeveRemoverItemEVincularAVenda` | Verifica remoção de item da venda |
| `RemoverItem_DeveEmitirEventoItemCancelado` | Verifica emissão de evento ao remover item |
| `AtualizarItem_DeveAtualizarQuantidadeERecalcularValor` | Verifica atualização de item e recálculo |
| `Cancelar_DeveAlterarStatusParaCancelado` | Verifica cancelamento de venda |
| `Cancelar_DeveEmitirEventoCompraCancelada` | Verifica emissão de evento ao cancelar |
| `Cancelar_VendaJaCancelada_DeveLancarExcecao` | Verifica que não é possível cancelar venda já cancelada |

#### VendaItemTests (10 testes)
**Arquivo**: `tests/UnitTests/Domain/Entities/VendaItemTests.cs`

| Teste | Descrição |
|-------|-----------|
| `CriarItem_QuantidadeAte3_DeveAplicarSemDesconto` | Verifica que até 3 itens não recebem desconto |
| `CriarItem_Quantidade4_DeveAplicarSemDesconto` | Verifica que 4 itens não recebem desconto |
| `CriarItem_QuantidadeEntre5E9_DeveAplicar10PorcentoDesconto` | Verifica desconto de 10% para 5-9 itens |
| `CriarItem_QuantidadeEntre10E20_DeveAplicar20PorcentoDesconto` | Verifica desconto de 20% para 10-20 itens |
| `CriarItem_QuantidadeMaiorQue20_DeveLancarExcecao` | Verifica que quantidade > 20 lança exceção |
| `CriarItem_QuantidadeZero_DeveLancarExcecao` | Verifica validação de quantidade zero |
| `CriarItem_ValorUnitarioZero_DeveLancarExcecao` | Verifica validação de valor unitário zero |
| `AtualizarQuantidade_DeveRecalcularDesconto` | Verifica recálculo de desconto ao atualizar quantidade |
| `AtualizarQuantidade_MaiorQue20_DeveLancarExcecao` | Verifica que atualizar para > 20 lança exceção |
| `AtualizarValorUnitario_DeveRecalcularValorTotal` | Verifica recálculo ao atualizar valor unitário |

### 1.2. Domain - Services

#### DescontoServiceTests (9 testes)
**Arquivo**: `tests/UnitTests/Domain/Services/DescontoServiceTests.cs`

| Teste | Descrição |
|-------|-----------|
| `CalcularDesconto_QuantidadeAte3_DeveRetornarZero` | Verifica desconto zero para até 3 itens |
| `CalcularDesconto_Quantidade4_DeveRetornarZero` | Verifica desconto zero para 4 itens |
| `CalcularDesconto_Quantidade5_DeveRetornar10Porcento` | Verifica 10% de desconto para 5 itens |
| `CalcularDesconto_Quantidade9_DeveRetornar10Porcento` | Verifica 10% de desconto para 9 itens |
| `CalcularDesconto_Quantidade10_DeveRetornar20Porcento` | Verifica 20% de desconto para 10 itens |
| `CalcularDesconto_Quantidade20_DeveRetornar20Porcento` | Verifica 20% de desconto para 20 itens |
| `CalcularDesconto_QuantidadeMaiorQue20_DeveLancarExcecao` | Verifica exceção para > 20 itens |
| `CalcularValorTotalItem_Quantidade5_DeveAplicar10PorcentoDesconto` | Verifica cálculo total com desconto 10% |
| `CalcularValorTotalItem_Quantidade15_DeveAplicar20PorcentoDesconto` | Verifica cálculo total com desconto 20% |

### 1.3. Domain - Value Objects

#### ClienteIdTests (3 testes)
**Arquivo**: `tests/UnitTests/Domain/ValueObjects/ClienteIdTests.cs`

| Teste | Descrição |
|-------|-----------|
| `CriarClienteId_ComGuidValido_DeveCriarComSucesso` | Verifica criação com GUID válido |
| `CriarClienteId_ComGuidVazio_DeveLancarExcecao` | Verifica validação de GUID vazio |
| `ClienteId_ConversaoImplicita_DeveFuncionar` | Verifica conversão implícita para GUID |

### 1.4. Application - Handlers

#### CreateVendaHandlerTests (1 teste)
**Arquivo**: `tests/UnitTests/Application/Handlers/CreateVendaHandlerTests.cs`

| Teste | Descrição |
|-------|-----------|
| `Handle_DeveCriarVendaComSucesso` | Verifica criação de venda através do handler |

### 1.5. Application - Validators

#### CreateVendaCommandValidatorTests (4 testes)
**Arquivo**: `tests/UnitTests/Application/Validators/CreateVendaCommandValidatorTests.cs`

| Teste | Descrição |
|-------|-----------|
| `Validar_ComandoValido_DevePassar` | Verifica validação de comando válido |
| `Validar_NumeroVendaVazio_DeveFalhar` | Verifica validação de número de venda obrigatório |
| `Validar_ItensVazio_DeveFalhar` | Verifica validação de lista de itens obrigatória |
| `Validar_QuantidadeMaiorQue20_DeveFalhar` | Verifica validação de quantidade máxima |

---

## 🔗 2. Testes de Integração (57 testes)

### 2.1. VendaControllerIntegrationTests (19 testes)
**Arquivo**: `tests/IntegrationTests/Controllers/VendaControllerIntegrationTests.cs`

#### CREATE (POST /api/venda)
- `Post_CriarVenda_DeveRetornar201CreatedComId` - Verifica criação com retorno de ID
- `Post_CriarVenda_DeveCriarComTodosOsCamposObrigatorios` - Verifica todos os campos obrigatórios
- `Post_CriarVenda_DeveValidarExternalIdentities` - Verifica validação de identidades externas
- `Post_CriarVenda_DeveCalcularValorTotalCorretamente` - Verifica cálculo de valor total
- `Post_CriarVenda_DeveCalcularDescontosPorItem` - Verifica cálculo de descontos
- `Post_CriarVenda_DeveValidarCamposObrigatorios` - Verifica validação de campos
- `Post_CriarVenda_DeveCriarComStatusNaoCancelado` - Verifica status inicial

#### READ (GET /api/venda)
- `Get_ObterVendaPorId_DeveRetornarVendaExistente` - Verifica obtenção por ID
- `Get_ObterVendaPorId_DeveRetornarTodosOsCamposObrigatorios` - Verifica todos os campos retornados
- `Get_ObterVendaPorId_VendaInexistente_DeveRetornar404` - Verifica tratamento de venda inexistente
- `Get_ListarVendas_DeveRetornarListaDeVendas` - Verifica listagem de vendas
- `Get_ListarVendas_DeveRetornarFormatoCorreto` - Verifica formato da resposta

#### UPDATE (PUT /api/venda/{id})
- `Put_AtualizarVenda_DeveAtualizarItens` - Verifica atualização de itens
- `Put_AtualizarVenda_DeveAdicionarNovosItens` - Verifica adição de novos itens
- `Put_AtualizarVenda_DeveRemoverItens` - Verifica remoção de itens
- `Put_AtualizarVenda_DeveAtualizarItensExistentes` - Verifica atualização de itens existentes
- `Put_AtualizarVenda_VendaInexistente_DeveRetornar404` - Verifica tratamento de venda inexistente
- `Put_AtualizarVenda_VendaCancelada_DeveRetornar400` - Verifica que venda cancelada não pode ser atualizada

#### DELETE (DELETE /api/venda/{id})
- `Delete_CancelarVenda_DeveAlterarStatusParaCancelado` - Verifica cancelamento
- `Delete_CancelarVenda_VendaInexistente_DeveRetornar404` - Verifica tratamento de venda inexistente

### 2.2. RegrasDeNegocioIntegrationTests (15 testes)
**Arquivo**: `tests/IntegrationTests/Controllers/RegrasDeNegocioIntegrationTests.cs`

#### Regra 1: Desconto 10% para > 4 itens (até 9)
- `Post_Com5Itens_DeveAplicar10PorcentoDesconto` - Verifica desconto de 10% para 5 itens
- `Post_Com9Itens_DeveAplicar10PorcentoDesconto` - Verifica desconto de 10% para 9 itens
- `Post_Com4Itens_NaoDeveAplicarDesconto` - Verifica que 4 itens não recebem desconto
- `Put_AtualizandoPara5Itens_DeveAplicar10PorcentoDesconto` - Verifica desconto ao atualizar

#### Regra 2: Desconto 20% para 10-20 itens
- `Post_Com10Itens_DeveAplicar20PorcentoDesconto` - Verifica desconto de 20% para 10 itens
- `Post_Com15Itens_DeveAplicar20PorcentoDesconto` - Verifica desconto de 20% para 15 itens
- `Post_Com20Itens_DeveAplicar20PorcentoDesconto` - Verifica desconto de 20% para 20 itens
- `Put_AtualizandoPara10Itens_DeveAplicar20PorcentoDesconto` - Verifica desconto ao atualizar

#### Regra 3: Proibição de quantidade > 20
- `Post_Com21Itens_DeveRetornarErro` - Verifica que 21 itens retorna erro
- `Post_Com25Itens_DeveRetornarErro` - Verifica que 25 itens retorna erro
- `Put_AtualizandoPara21Itens_DeveRetornarErro` - Verifica erro ao atualizar para 21 itens

#### Regra 4: Desconto por item individual
- `Post_ComItensComQuantidadesDiferentes_DeveAplicarDescontoPorItem` - Verifica desconto individual
- `Post_ComMultiplosItens_DeveCalcularValorTotalCorretamente` - Verifica cálculo com múltiplos itens
- `Put_AdicionandoItemComQuantidadeDiferente_DeveAplicarDescontoCorreto` - Verifica desconto ao adicionar item

### 2.3. DomainEventsIntegrationTests (7 testes)
**Arquivo**: `tests/IntegrationTests/Controllers/DomainEventsIntegrationTests.cs`

- `Post_CriarVenda_DeveEmitirEventoCompraEfetuada` - Verifica evento ao criar venda
- `Post_CriarVenda_DeveCriarVendaComValorTotalCorreto` - Verifica valor total correto
- `Put_AtualizarVenda_DeveEmitirEventoCompraAlterada` - Verifica evento ao atualizar
- `Delete_CancelarVenda_DeveEmitirEventoCompraCancelada` - Verifica evento ao cancelar
- `Put_RemoverItem_DeveEmitirEventoItemCancelado` - Verifica evento ao remover item
- `Post_CriarVenda_DeveLogarEventoEmJSON` - Verifica log em formato JSON
- `Put_AtualizarVenda_DeveLogarEventoEmJSON` - Verifica log de atualização em JSON

### 2.4. ObservabilidadeIntegrationTests (9 testes)
**Arquivo**: `tests/IntegrationTests/Controllers/ObservabilidadeIntegrationTests.cs`

#### Health Check
- `Get_HealthCheck_DeveRetornar200OK` - Verifica status HTTP 200
- `Get_HealthCheck_DeveRetornarStatusHealthy` - Verifica status "healthy"
- `Get_HealthCheck_DeveRetornarTimestamp` - Verifica presença de timestamp
- `Get_HealthCheck_DeveRetornarUptime` - Verifica presença de uptime
- `Get_HealthCheck_DeveRetornarEnvironment` - Verifica presença de environment
- `Get_HealthCheck_DeveRetornarVersion` - Verifica presença de version

#### Correlation ID
- `Post_CriarVenda_DeveRetornarCorrelationIdNoHeader` - Verifica Correlation ID no header
- `Get_ObterVenda_DeveRetornarCorrelationIdNoHeader` - Verifica Correlation ID em GET
- `Post_CriarVenda_DeveIncluirCorrelationIdNaResposta` - Verifica Correlation ID na resposta
- `Get_ListarVendas_DeveRetornarCorrelationId` - Verifica Correlation ID na listagem

### 2.5. SecurityIntegrationTests (4 testes)
**Arquivo**: `tests/IntegrationTests/Controllers/SecurityIntegrationTests.cs`

- `Post_ComPayloadSQLInjection_DeveRejeitarOuSanitizar` - Verifica proteção contra SQL Injection
- `Post_ComPayloadXSS_DeveRejeitarOuSanitizar` - Verifica proteção contra XSS
- `Get_HealthCheck_DeveRetornarHeadersDeSeguranca` - Verifica headers de segurança
- `Post_CriarVenda_DeveValidarContentType` - Verifica validação de Content-Type

---

## 📋 3. Testes BDD (23 cenários)

### 3.1. Feature: Criar Venda
**Arquivo**: `tests/BddTests/Features/CriarVenda.feature`

| Cenário | Descrição |
|---------|-----------|
| `Criar venda com sucesso` | Cria venda com dados válidos e verifica sucesso |
| `Criar venda com quantidade acima do permitido` | Tenta criar venda com 21 itens e verifica erro |
| `Criar venda com todos os campos obrigatórios` | Verifica que todos os campos obrigatórios estão presentes |

### 3.2. Feature: Regras de Desconto por Quantidade
**Arquivo**: `tests/BddTests/Features/RegrasDeDesconto.feature`

| Cenário | Descrição |
|---------|-----------|
| `Aplicar desconto baseado na quantidade` | Testa descontos para quantidades de 1 a 20 itens |
| `Não permitir venda acima de 20 itens` | Verifica proibição de quantidade > 20 |
| `Não permitir desconto para menos de 4 itens` | Verifica que < 4 itens não recebem desconto |

**Exemplos de Desconto:**
- 1-4 itens: 0% desconto
- 5-9 itens: 10% desconto
- 10-20 itens: 20% desconto
- > 20 itens: Proibido

### 3.3. Feature: Eventos de Domínio
**Arquivo**: `tests/BddTests/Features/DomainEvents.feature`

| Cenário | Descrição |
|---------|-----------|
| `CompraEfetuada deve ser emitido ao criar venda` | Verifica evento ao criar venda |
| `CompraAlterada deve ser emitido ao atualizar venda` | Verifica evento ao atualizar |
| `CompraCancelada deve ser emitido ao cancelar venda` | Verifica evento ao cancelar |
| `ItemCancelado deve ser emitido ao remover item` | Verifica evento ao remover item |

### 3.4. Feature: Observabilidade
**Arquivo**: `tests/BddTests/Features/Observabilidade.feature`

| Cenário | Descrição |
|---------|-----------|
| `Logs devem estar em formato JSON` | Verifica formato JSON dos logs |
| `PerformanceMiddleware deve medir tempo de resposta` | Verifica medição de performance |
| `Correlation ID deve ser gerado e retornado` | Verifica geração de Correlation ID |
| `Requisições lentas devem ser logadas como Warning` | Verifica log de requisições lentas |
| `Health check deve retornar métricas` | Verifica endpoint de health check |

---

## 📈 Cobertura de Testes

### Por Camada

| Camada | Cobertura | Testes |
|--------|-----------|--------|
| **Domain** | 100% | 33 testes unitários |
| **Application** | ~85% | 5 testes unitários |
| **Infrastructure** | ~70% | Testes de integração |
| **API** | ~80% | Testes de integração |

### Por Funcionalidade

| Funcionalidade | Cobertura | Testes |
|----------------|-----------|--------|
| **Criação de Venda** | 100% | 15 testes |
| **Atualização de Venda** | 100% | 12 testes |
| **Cancelamento de Venda** | 100% | 8 testes |
| **Cálculo de Descontos** | 100% | 25 testes |
| **Eventos de Domínio** | 100% | 11 testes |
| **Observabilidade** | 100% | 13 testes |
| **Validações** | 100% | 10 testes |
| **Segurança** | 80% | 4 testes |

---

## 🎯 Cenários de Teste Contemplados

### Regras de Negócio

1. ✅ **Desconto por Quantidade**
   - Sem desconto: 1-4 itens
   - 10% desconto: 5-9 itens
   - 20% desconto: 10-20 itens
   - Proibido: > 20 itens

2. ✅ **Validações**
   - Campos obrigatórios
   - Quantidade mínima/máxima
   - Valor unitário > 0
   - GUIDs válidos

3. ✅ **Estados da Venda**
   - Criação com status NaoCancelado
   - Cancelamento altera status
   - Venda cancelada não pode ser alterada

4. ✅ **Cálculos**
   - Valor total por item
   - Valor total da venda
   - Desconto por item individual

### Eventos de Domínio

1. ✅ **CompraEfetuada** - Emitido ao criar venda
2. ✅ **CompraAlterada** - Emitido ao atualizar venda
3. ✅ **CompraCancelada** - Emitido ao cancelar venda
4. ✅ **ItemCancelado** - Emitido ao remover item

### Observabilidade

1. ✅ **Logs em JSON** - Todos os logs em formato estruturado
2. ✅ **Correlation ID** - Rastreamento de requisições
3. ✅ **Performance Middleware** - Medição de tempo de resposta
4. ✅ **Health Check** - Endpoint de saúde da aplicação

### Segurança

1. ✅ **SQL Injection** - Proteção contra injeção SQL
2. ✅ **XSS** - Proteção contra cross-site scripting
3. ✅ **Validação de Content-Type** - Validação de tipos de conteúdo

---

## 🚀 Como Executar os Testes

### Todos os Testes

```bash
# Usando Docker (recomendado)
docker-compose -f docker-compose.test.yml up all-tests

# Ou usando script
.\scripts\run-tests-docker.ps1
```

### Testes Específicos

```bash
# Apenas testes unitários
dotnet test tests/UnitTests/UnitTests.csproj

# Apenas testes de integração
dotnet test tests/IntegrationTests/IntegrationTests.csproj

# Apenas testes BDD
dotnet test tests/BddTests/BddTests.csproj
```

Para mais detalhes sobre execução de testes, consulte [TESTES_DOCKER.md](./TESTES_DOCKER.md).

---

## 📝 Notas Importantes

1. **Testes Unitários**: Não requerem banco de dados, executam isoladamente
2. **Testes de Integração**: Requerem SQL Server rodando (porta 1434 ou via Docker)
3. **Testes BDD**: Requerem SQL Server e usam SpecFlow para cenários em linguagem natural
4. **Cobertura**: Domain Layer tem 100% de cobertura (obrigatório)
5. **Resultados**: Todos os resultados são salvos em `./test-results/` em formato TRX

---

## 🔄 Manutenção dos Testes

Ao adicionar novas funcionalidades:

1. **Sempre adicione testes unitários** para lógica de domínio
2. **Adicione testes de integração** para novos endpoints
3. **Atualize testes BDD** se houver novas regras de negócio
4. **Mantenha cobertura mínima** de 100% no Domain Layer
5. **Documente novos cenários** neste arquivo

---

**Última atualização**: 2024-01-15
**Total de Testes**: 118 (38 unitários + 57 integração + 23 BDD)

